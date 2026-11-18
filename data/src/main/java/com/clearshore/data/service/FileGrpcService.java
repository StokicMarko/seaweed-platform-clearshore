package com.clearshore.data.service;

import com.clearshore.data.generated.FileDataGrpc;
import com.clearshore.data.generated.FileProto;
import com.clearshore.data.storage.FileStorage;
import com.google.protobuf.Empty;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import org.springframework.grpc.server.service.GrpcService;

import java.io.IOException;
import java.io.OutputStream;

@GrpcService
public class FileGrpcService extends FileDataGrpc.FileDataImplBase {

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
            case ORIGINAL_FILE_NAME -> {
              if (output != null) {
                fail(Status.INVALID_ARGUMENT.withDescription("The file name was sent twice"));
                return;
              }
              storedFileName = fileStorage.newStoredFileName(request.getOriginalFileName());
              output = fileStorage.create(storedFileName);
            }
            case CHUNK -> {
              if (output == null) {
                fail(Status.INVALID_ARGUMENT.withDescription("The first message must carry the file name"));
                return;
              }
              request.getChunk().writeTo(output);
            }
            case PART_NOT_SET ->
                fail(Status.INVALID_ARGUMENT.withDescription("Empty message"));
          }
        } catch (IOException | RuntimeException e) {
          fail(Status.INTERNAL.withDescription(e.getMessage()));
        }
      }

      @Override
      public void onError(Throwable t) {
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

      private void fail(Status status) {
        failed = true;
        discard();
        response.onError(status.asRuntimeException());
      }

      /** Closes and removes a half-written file. */
      private void discard() {
        try {
          if (output != null) {
            output.close();
          }
          if (storedFileName != null) {
            fileStorage.delete(storedFileName);
          }
        } catch (IOException ignored) {
          // best effort
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
}