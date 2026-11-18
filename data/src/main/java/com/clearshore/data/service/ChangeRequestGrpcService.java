package com.clearshore.data.service;

import com.clearshore.data.entity.ChangeRequest;
import com.clearshore.data.generated.ChangeRequestDataGrpc;
import com.clearshore.data.generated.ChangeRequestProto;
import com.clearshore.data.mapper.ChangeRequestMapper;
import com.clearshore.data.repository.ChangeRequestRepository;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.grpc.server.service.GrpcService;

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
}