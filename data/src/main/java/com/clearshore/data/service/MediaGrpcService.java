package com.clearshore.data.service;

import com.clearshore.data.entity.Media;
import com.clearshore.data.generated.MediaDataGrpc;
import com.clearshore.data.generated.MediaProto;
import com.clearshore.data.mapper.MediaMapper;
import com.clearshore.data.repository.MediaRepository;
import com.clearshore.data.storage.FileStorage;
import com.google.protobuf.ByteString;
import io.grpc.Status;
import io.grpc.StatusRuntimeException;
import io.grpc.stub.StreamObserver;
import org.springframework.grpc.server.service.GrpcService;

import java.io.IOException;
import java.io.InputStream;
import java.nio.file.NoSuchFileException;
import java.util.List;
import java.util.Optional;

@GrpcService
public class MediaGrpcService extends MediaDataGrpc.MediaDataImplBase {

    private static final int CHUNK_SIZE = 64 * 1024;

    private final MediaRepository mediaRepository;
    private final FileStorage fileStorage;

    public MediaGrpcService(MediaRepository mediaRepository, FileStorage fileStorage) {
        this.mediaRepository = mediaRepository;
        this.fileStorage = fileStorage;
    }

    @Override
    public void getMany(MediaProto.GetManyMediaRequest request,
                        StreamObserver<MediaProto.MediaList> responseObserver) {
        List<Media> media = request.hasMachineCode()
                ? mediaRepository.findByMachineCodeOrderByPublishedAtDesc(request.getMachineCode())
                : mediaRepository.findAllByOrderByPublishedAtDesc();
        responseObserver.onNext(MediaMapper.toProtoList(media));
        responseObserver.onCompleted();
    }

    @Override
    public void getSingle(MediaProto.MediaId request, StreamObserver<MediaProto.Media> responseObserver) {
        Optional<Media> media = mediaRepository.findById(request.getId());
        if (media.isEmpty()) {
            responseObserver.onError(notFound("Media " + request.getId() + " not found"));
            return;
        }
        responseObserver.onNext(MediaMapper.toProto(media.get()));
        responseObserver.onCompleted();
    }

    @Override
    public void readFile(MediaProto.MediaId request, StreamObserver<MediaProto.FileChunk> responseObserver) {
        Optional<Media> media = mediaRepository.findById(request.getId());
        if (media.isEmpty()) {
            responseObserver.onError(notFound("Media " + request.getId() + " not found"));
            return;
        }
        try (InputStream input = fileStorage.open(media.get().getStoredFileName())) {
            byte[] buffer = new byte[CHUNK_SIZE];
            int read;
            while ((read = input.read(buffer)) != -1) {
                responseObserver.onNext(MediaProto.FileChunk.newBuilder()
                        .setData(ByteString.copyFrom(buffer, 0, read))
                        .build());
            }
            responseObserver.onCompleted();
        } catch (NoSuchFileException e) {
            responseObserver.onError(notFound("The file of media " + request.getId() + " is missing"));
        } catch (IOException e) {
            responseObserver.onError(Status.INTERNAL.withDescription(e.getMessage()).asRuntimeException());
        }
    }

    private static StatusRuntimeException notFound(String message) {
        return Status.NOT_FOUND.withDescription(message).asRuntimeException();
    }
}
