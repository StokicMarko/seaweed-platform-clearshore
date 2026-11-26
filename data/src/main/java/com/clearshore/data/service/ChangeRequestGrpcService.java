package com.clearshore.data.service;

import com.clearshore.data.entity.ChangeRequest;
import com.clearshore.data.entity.ChangeStatus;
import com.clearshore.data.generated.ChangeRequestDataGrpc;
import com.clearshore.data.generated.ChangeRequestProto;
import com.clearshore.data.mapper.ChangeRequestMapper;
import com.clearshore.data.repository.ChangeRequestRepository;
import com.google.protobuf.Empty;
import io.grpc.Status;
import io.grpc.StatusRuntimeException;
import io.grpc.stub.StreamObserver;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.grpc.server.service.GrpcService;

import java.util.List;
import java.util.Optional;

@GrpcService
public class ChangeRequestGrpcService extends ChangeRequestDataGrpc.ChangeRequestDataImplBase {

    private static final Logger log = LoggerFactory.getLogger(ChangeRequestGrpcService.class);

    private final ChangeRequestRepository changeRequestRepository;

    public ChangeRequestGrpcService(ChangeRequestRepository changeRequestRepository) {
        this.changeRequestRepository = changeRequestRepository;
    }

    @Override
    public void create(ChangeRequestProto.ChangeRequest request,
        StreamObserver<ChangeRequestProto.ChangeRequest> responseObserver) {
        try {
            ChangeRequest saved = changeRequestRepository.save(ChangeRequestMapper.toEntity(request));
            responseObserver.onNext(ChangeRequestMapper.toProto(saved));
            responseObserver.onCompleted();
        } catch (IllegalArgumentException | DataIntegrityViolationException e) {
            responseObserver.onError(Status.INVALID_ARGUMENT.withDescription(e.getMessage()).asRuntimeException());
        } catch (RuntimeException e) {
            log.error("Could not create the change request", e);
            responseObserver.onError(Status.INTERNAL
                .withDescription("Could not create the change request")
                .asRuntimeException());
        }
    }

    @Override
    public void getMany(ChangeRequestProto.GetManyChangeRequestsRequest request,
        StreamObserver<ChangeRequestProto.ChangeRequestList> out) {
        try {
            List<ChangeRequest> found = changeRequestRepository.findFiltered(
                request.hasTargetType() ? ChangeRequestMapper.toEntity(request.getTargetType()) : null,
                request.hasTargetId() ? request.getTargetId() : null,
                request.hasStatus() ? ChangeRequestMapper.toEntity(request.getStatus()) : null);
            out.onNext(ChangeRequestMapper.toProtoList(found));
            out.onCompleted();
        } catch (IllegalArgumentException e) {
            out.onError(Status.INVALID_ARGUMENT.withDescription(e.getMessage()).asRuntimeException());
        } catch (RuntimeException e) {
            log.error("Could not list the change requests", e);
            out.onError(Status.INTERNAL.withDescription("Could not list the change requests").asRuntimeException());
        }
    }

    @Override
    public void getSingle(ChangeRequestProto.ChangeRequestId request,
        StreamObserver<ChangeRequestProto.ChangeRequest> out) {
        try {
            Optional<ChangeRequest> found = changeRequestRepository.findById(request.getId());
            if (found.isEmpty()) {
                out.onError(notFound(request.getId()));
                return;
            }
            out.onNext(ChangeRequestMapper.toProto(found.get()));
            out.onCompleted();
        } catch (RuntimeException e) {
            log.error("Could not get the change request", e);
            out.onError(Status.INTERNAL.withDescription("Could not get the change request").asRuntimeException());
        }
    }

    @Override
    public void updateStatus(ChangeRequestProto.UpdateChangeRequestStatusRequest request,
        StreamObserver<ChangeRequestProto.ChangeRequest> out) {
        try {
            ChangeStatus status = ChangeRequestMapper.toEntity(request.getStatus());
            Optional<ChangeRequest> found = changeRequestRepository.findById(request.getId());
            if (found.isEmpty()) {
                out.onError(notFound(request.getId()));
                return;
            }
            found.get().updateStatus(status);
            out.onNext(ChangeRequestMapper.toProto(changeRequestRepository.save(found.get())));
            out.onCompleted();
        } catch (IllegalArgumentException e) {
            out.onError(Status.INVALID_ARGUMENT.withDescription(e.getMessage()).asRuntimeException());
        } catch (RuntimeException e) {
            log.error("Could not update the change request status", e);
            out.onError(Status.INTERNAL
                .withDescription("Could not update the change request status").asRuntimeException());
        }
    }

    @Override
    public void delete(ChangeRequestProto.ChangeRequestId request, StreamObserver<Empty> out) {
        try {
            if (!changeRequestRepository.existsById(request.getId())) {
                out.onError(notFound(request.getId()));
                return;
            }
            changeRequestRepository.deleteById(request.getId());
            out.onNext(Empty.getDefaultInstance());
            out.onCompleted();
        } catch (RuntimeException e) {
            log.error("Could not delete the change request", e);
            out.onError(Status.INTERNAL.withDescription("Could not delete the change request").asRuntimeException());
        }
    }

    private static StatusRuntimeException notFound(int id) {
        return Status.NOT_FOUND.withDescription("Change request " + id + " not found").asRuntimeException();
    }
}