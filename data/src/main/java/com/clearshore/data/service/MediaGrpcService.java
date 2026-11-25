package com.clearshore.data.service;

import com.clearshore.data.entity.MediaItem;
import com.clearshore.data.entity.Technology;
import com.clearshore.data.generated.CommonProto;
import com.clearshore.data.generated.MediaDataGrpc;
import com.clearshore.data.generated.MediaProto;
import com.clearshore.data.mapper.MediaMapper;
import com.clearshore.data.repository.MediaRepository;
import com.clearshore.data.repository.TechnologyRepository;
import com.clearshore.data.storage.FileStorage;
import com.google.protobuf.ByteString;
import com.google.protobuf.Empty;
import io.grpc.Status;
import io.grpc.StatusRuntimeException;
import io.grpc.stub.StreamObserver;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.grpc.server.service.GrpcService;

import java.io.IOException;
import java.io.InputStream;
import java.nio.file.NoSuchFileException;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Optional;
import java.util.Set;

@GrpcService
public class MediaGrpcService extends MediaDataGrpc.MediaDataImplBase {

    private static final Logger log = LoggerFactory.getLogger(MediaGrpcService.class);
    private static final int CHUNK_SIZE = 64 * 1024;

    private final MediaRepository mediaRepository;
    private final TechnologyRepository technologyRepository;
    private final FileStorage fileStorage;

    public MediaGrpcService(MediaRepository mediaRepository,
        TechnologyRepository technologyRepository,
        FileStorage fileStorage) {
        this.mediaRepository = mediaRepository;
        this.technologyRepository = technologyRepository;
        this.fileStorage = fileStorage;
    }

    @Override
    public void getMany(MediaProto.GetManyMediaRequest request,
        StreamObserver<MediaProto.MediaList> responseObserver) {
        List<MediaItem> mediaItems = request.hasTechnologyType()
            ? mediaRepository.findByTechnologyType(request.getTechnologyType())
            : mediaRepository.findAllByOrderByPublishedAtDesc();
        responseObserver.onNext(MediaMapper.toProtoList(mediaItems));
        responseObserver.onCompleted();
    }

    @Override
    public void getSingle(MediaProto.MediaId request, StreamObserver<MediaProto.Media> responseObserver) {
        Optional<MediaItem> media = mediaRepository.findById(request.getId());
        if (media.isEmpty()) {
            responseObserver.onError(notFound("Media " + request.getId() + " not found"));
            return;
        }
        responseObserver.onNext(MediaMapper.toProto(media.get()));
        responseObserver.onCompleted();
    }

    @Override
    public void readFile(MediaProto.MediaId request, StreamObserver<CommonProto.FileChunk> responseObserver) {
        Optional<MediaItem> media = mediaRepository.findById(request.getId());
        if (media.isEmpty()) {
            responseObserver.onError(notFound("Media " + request.getId() + " not found"));
            return;
        }
        try (InputStream input = fileStorage.open(media.get().getStoredFileName())) {
            byte[] buffer = new byte[CHUNK_SIZE];
            int read;
            while ((read = input.read(buffer)) != -1) {
                responseObserver.onNext(CommonProto.FileChunk.newBuilder()
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

    // Safe to repeat: if a media item already uses this stored_file_name, it is returned and nothing is created.
    @Override
    public void create(MediaProto.CreateMediaRequest r, StreamObserver<MediaProto.Media> out) {
        try {
            Optional<MediaItem> existing = mediaRepository.findByStoredFileName(r.getStoredFileName());
            if (existing.isPresent()) {
                reply(out, existing.get());
                return;
            }
            CommonProto.LocalizedText title = requireTitle(r.hasTitle(), r.getTitle());
            boolean hasDescription = r.hasDescription();
            MediaItem saved = mediaRepository.save(new MediaItem(
                technologies(r.getTechnologyTypesList()),
                MediaMapper.toEntity(r.getMediaType()),
                title.getEn(), title.getEs(), title.getFr(),
                hasDescription ? r.getDescription().getEn() : null,
                hasDescription ? r.getDescription().getEs() : null,
                hasDescription ? r.getDescription().getFr() : null,
                r.getOriginalFileName(), r.getStoredFileName(), r.getContentType()));
            reply(out, saved);
        } catch (DataIntegrityViolationException e) {
            // Two identical creates at the same time: the other one won, so return its item.
            Optional<MediaItem> winner = mediaRepository.findByStoredFileName(r.getStoredFileName());
            if (winner.isPresent()) {
                reply(out, winner.get());
            } else {
                out.onError(Status.INVALID_ARGUMENT.withDescription(e.getMessage()).asRuntimeException());
            }
        } catch (IllegalArgumentException e) {
            out.onError(Status.INVALID_ARGUMENT.withDescription(e.getMessage()).asRuntimeException());
        } catch (RuntimeException e) {
            log.error("Could not create the media item", e);
            out.onError(Status.INTERNAL.withDescription("Could not create the media item").asRuntimeException());
        }
    }

    // Replaces the data. The four file fields are all set (new file) or all not set (keep the current file).
    // Description not set = no description.
    @Override
    public void update(MediaProto.UpdateMediaRequest r, StreamObserver<MediaProto.Media> out) {
        try {
            int filesSet = (r.hasMediaType() ? 1 : 0)
                + (r.hasOriginalFileName() ? 1 : 0)
                + (r.hasStoredFileName() ? 1 : 0)
                + (r.hasContentType() ? 1 : 0);
            if (filesSet != 0 && filesSet != 4) {
                throw new IllegalArgumentException("The four file fields must be all set or all not set");
            }
            CommonProto.LocalizedText title = requireTitle(r.hasTitle(), r.getTitle());
            Set<Technology> technologies = technologies(r.getTechnologyTypesList());

            Optional<MediaItem> found = mediaRepository.findById(r.getId());
            if (found.isEmpty()) {
                out.onError(notFound("Media " + r.getId() + " not found"));
                return;
            }
            MediaItem media = found.get();
            boolean hasDescription = r.hasDescription();
            media.updateDetails(technologies,
                title.getEn(), title.getEs(), title.getFr(),
                hasDescription ? r.getDescription().getEn() : null,
                hasDescription ? r.getDescription().getEs() : null,
                hasDescription ? r.getDescription().getFr() : null);
            if (filesSet == 4) {
                media.replaceFile(MediaMapper.toEntity(r.getMediaType()), r.getOriginalFileName(),
                    r.getStoredFileName(), r.getContentType());
            }
            reply(out, mediaRepository.saveAndFlush(media));
        } catch (DataIntegrityViolationException | IllegalArgumentException e) {
            out.onError(Status.INVALID_ARGUMENT.withDescription(e.getMessage()).asRuntimeException());
        } catch (RuntimeException e) {
            log.error("Could not update the media item", e);
            out.onError(Status.INTERNAL.withDescription("Could not update the media item").asRuntimeException());
        }
    }

    // Deletes the row only. The stored file is deleted by .NET through FileData.Delete.
    @Override
    public void delete(MediaProto.MediaId r, StreamObserver<Empty> out) {
        try {
            Optional<MediaItem> found = mediaRepository.findById(r.getId());
            if (found.isEmpty()) {
                out.onError(notFound("Media " + r.getId() + " not found"));
                return;
            }
            mediaRepository.delete(found.get());
            out.onNext(Empty.getDefaultInstance());
            out.onCompleted();
        } catch (RuntimeException e) {
            log.error("Could not delete the media item", e);
            out.onError(Status.INTERNAL.withDescription("Could not delete the media item").asRuntimeException());
        }
    }

    private Set<Technology> technologies(List<String> types) {
        Set<String> wanted = new LinkedHashSet<>(types);
        List<Technology> found = technologyRepository.findAllById(wanted);
        if (found.size() != wanted.size()) {
            throw new IllegalArgumentException("Unknown technology type");
        }
        return new LinkedHashSet<>(found);
    }

    private static CommonProto.LocalizedText requireTitle(boolean hasTitle, CommonProto.LocalizedText title) {
        if (!hasTitle) {
            throw new IllegalArgumentException("A media item needs a title");
        }
        return title;
    }

    private static void reply(StreamObserver<MediaProto.Media> out, MediaItem item) {
        out.onNext(MediaMapper.toProto(item));
        out.onCompleted();
    }

    private static StatusRuntimeException notFound(String message) {
        return Status.NOT_FOUND.withDescription(message).asRuntimeException();
    }
}