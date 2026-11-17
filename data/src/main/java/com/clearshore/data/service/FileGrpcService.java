package com.clearshore.data.service;

import com.clearshore.data.generated.CommonProto;
import com.clearshore.data.generated.FileDataGrpc;
import com.clearshore.data.generated.FileProto;
import com.clearshore.data.storage.FileStorage;
import com.google.protobuf.ByteString;
import com.google.protobuf.Empty;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import org.springframework.grpc.server.service.GrpcService;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.file.NoSuchFileException;

@GrpcService
public class FileGrpcService extends FileDataGrpc.FileDataImplBase {

    private static final int CHUNK_SIZE = 64 * 1024;

    private final FileStorage fileStorage;

    public FileGrpcService(FileStorage fileStorage) {
        this.fileStorage = fileStorage;
    }

    @Override
    public StreamObserver<FileProto.SaveFileRequest> save(StreamObserver<FileProto.StoredFile> response) {
        return new StreamObserver<>() {
            private String storedFileName;
            private OutputStream output;
            private boolean failed;

            @Override
            public void onNext(FileProto.SaveFileRequest request) {
                if (failed) {
                    return;
                }
                try {
                    switch (request.getPartCase()) {
                        case ORIGINAL_FILE_NAME -> start(request.getOriginalFileName());
                        case CHUNK -> write(request);
                        case PART_NOT_SET -> fail(Status.INVALID_ARGUMENT.withDescription("Empty message"));
                    }
                } catch (IOException | RuntimeException e) {
                    fail(Status.INTERNAL.withDescription(e.getMessage()));
                }
            }

            @Override
            public void onError(Throwable error) {
                discard();
            }

            @Override
            public void onCompleted() {
                if (failed) {
                    return;
                }
                if (output == null) {
                    fail(Status.INVALID_ARGUMENT.withDescription("No file was sent"));
                    return;
                }
                try {
                    output.close();
                    response.onNext(FileProto.StoredFile.newBuilder().setStoredFileName(storedFileName).build());
                    response.onCompleted();
                } catch (IOException e) {
                    fail(Status.INTERNAL.withDescription(e.getMessage()));
                }
            }

            private void start(String originalFileName) throws IOException {
                if (output != null) {
                    fail(Status.INVALID_ARGUMENT.withDescription("The file name was sent twice"));
                    return;
                }
                storedFileName = fileStorage.newStoredFileName(originalFileName);
                output = fileStorage.create(storedFileName);
            }

            private void write(FileProto.SaveFileRequest request) throws IOException {
                if (output == null) {
                    fail(Status.INVALID_ARGUMENT.withDescription("The first message must carry the file name"));
                    return;
                }
                request.getChunk().writeTo(output);
            }

            private void fail(Status status) {
                failed = true;
                discard();
                response.onError(status.asRuntimeException());
            }

            private void discard() {
                try {
                    if (output != null) {
                        output.close();
                    }
                    if (storedFileName != null) {
                        fileStorage.delete(storedFileName);
                    }
                } catch (IOException ignored) {
                }
            }
        };
    }

    @Override
    public void delete(FileProto.StoredFile request, StreamObserver<Empty> response) {
        try {
            fileStorage.delete(request.getStoredFileName());
            response.onNext(Empty.getDefaultInstance());
            response.onCompleted();
        } catch (IllegalArgumentException e) {
            response.onError(Status.INVALID_ARGUMENT.withDescription(e.getMessage()).asRuntimeException());
        } catch (IOException e) {
            response.onError(Status.INTERNAL.withDescription(e.getMessage()).asRuntimeException());
        }
    }

    @Override
    public void read(FileProto.StoredFile request, StreamObserver<CommonProto.FileChunk> response) {
        try (InputStream input = fileStorage.open(request.getStoredFileName())) {
            byte[] buffer = new byte[CHUNK_SIZE];
            int read;
            while ((read = input.read(buffer)) != -1) {
                response.onNext(CommonProto.FileChunk.newBuilder()
                        .setData(ByteString.copyFrom(buffer, 0, read))
                        .build());
            }
            response.onCompleted();
        } catch (NoSuchFileException e) {
            response.onError(Status.NOT_FOUND
                    .withDescription("File " + request.getStoredFileName() + " not found").asRuntimeException());
        } catch (IllegalArgumentException e) {
            response.onError(Status.INVALID_ARGUMENT.withDescription(e.getMessage()).asRuntimeException());
        } catch (IOException e) {
            response.onError(Status.INTERNAL.withDescription(e.getMessage()).asRuntimeException());
        }
    }
}
