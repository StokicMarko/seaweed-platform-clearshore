package com.clearshore.data.controller;

import com.clearshore.data.grpc.*;
import com.clearshore.data.mapper.MediaMapper;
import com.clearshore.data.service.MediaService;
import io.grpc.Status;
import io.grpc.StatusRuntimeException;
import io.grpc.stub.StreamObserver;
import org.springframework.grpc.server.service.GrpcService;

import java.io.IOException;
import java.io.OutputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.NoSuchElementException;

    @GrpcService
    public class MediaGrpcController extends MediaDataGrpc.MediaDataImplBase {

        private final MediaService service;

        public MediaGrpcController(MediaService service) {
            this.service = service;
        }

        @Override
        public StreamObserver<SaveMediaChunk> saveMedia(StreamObserver<MediaItem> out) {
            return new StreamObserver<>() {
                private MediaMetadata metadata;
                private Path file;
                private OutputStream stream;
                private boolean failed;

                @Override
                public void onNext(SaveMediaChunk message) {
                    if (failed) return;
                    try {
                        switch (message.getDataCase()) {
                            case METADATA -> {
                                if (metadata != null) {
                                    throw new IllegalArgumentException("metadata was already sent");
                                }
                                metadata = message.getMetadata();
                                file = service.prepareFile(metadata.getFileName());
                                stream = Files.newOutputStream(file);
                            }
                            case CHUNK -> {
                                if (stream == null) {
                                    throw new IllegalArgumentException("the first message must be metadata");
                                }
                                stream.write(message.getChunk().toByteArray());
                            }
                            case DATA_NOT_SET -> { }
                        }
                    } catch (Exception e) {
                        fail(e);
                    }
                }

                @Override
                public void onError(Throwable t) {   // the client cancelled or broke
                    failed = true;
                    cleanup();
                }

                @Override
                public void onCompleted() {
                    if (failed) return;
                    try {
                        if (metadata == null) {
                            throw new IllegalArgumentException("no metadata received");
                        }
                        stream.close();
                        var saved = service.create(MediaMapper.toDomain(metadata), file);
                        out.onNext(MediaMapper.toProto(saved));
                        out.onCompleted();
                    } catch (Exception e) {
                        fail(e);
                    }
                }

                private void fail(Exception e) {
                    failed = true;
                    cleanup();
                    out.onError(toStatus(e));
                }

                private void cleanup() {
                    try {
                        if (stream != null) stream.close();
                    } catch (IOException ignored) {
                    }
                    service.discard(file);
                }
            };
        }

        @Override
        public void updateMedia(UpdateMediaRequest request, StreamObserver<MediaItem> out) {
            try {
                var changes = MediaMapper.toDomain(request.getMetadata());
                out.onNext(MediaMapper.toProto(service.update(request.getId(), changes)));
                out.onCompleted();
            } catch (Exception e) {
                out.onError(toStatus(e));
            }
        }

        @Override
        public void listMedia(ListMediaRequest request, StreamObserver<ListMediaReply> out) {
            try {
                var items = service.list(request.getMachine()).stream()
                        .map(MediaMapper::toProto).toList();
                out.onNext(ListMediaReply.newBuilder().addAllItems(items).build());
                out.onCompleted();
            } catch (Exception e) {
                out.onError(toStatus(e));
            }
        }

        @Override
        public void getMedia(MediaIdRequest request, StreamObserver<MediaItem> out) {
            try {
                out.onNext(MediaMapper.toProto(service.get(request.getId())));
                out.onCompleted();
            } catch (Exception e) {
                out.onError(toStatus(e));
            }
        }

        @Override
        public void deleteMedia(MediaIdRequest request, StreamObserver<DeleteReply> out) {
            try {
                boolean ok = service.delete(request.getId());
                out.onNext(DeleteReply.newBuilder().setSuccess(ok).build());
                out.onCompleted();
            } catch (Exception e) {
                out.onError(toStatus(e));
            }
        }

        private static StatusRuntimeException toStatus(Exception e) {
            if (e instanceof StatusRuntimeException s) return s;
            Status status = Status.INTERNAL;
            if (e instanceof IllegalArgumentException) status = Status.INVALID_ARGUMENT;
            else if (e instanceof NoSuchElementException) status = Status.NOT_FOUND;
            return status.withDescription(e.getMessage()).asRuntimeException();
        }
    }
