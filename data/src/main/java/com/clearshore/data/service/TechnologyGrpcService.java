package com.clearshore.data.service;

import com.clearshore.data.generated.TechnologyDataGrpc;
import com.clearshore.data.generated.TechnologyProto;
import com.clearshore.data.mapper.TechnologyMapper;
import com.clearshore.data.repository.TechnologyRepository;
import io.grpc.stub.StreamObserver;
import org.springframework.grpc.server.service.GrpcService;

@GrpcService
public class TechnologyGrpcService extends TechnologyDataGrpc.TechnologyDataImplBase {

    private final TechnologyRepository technologyRepository;

    public TechnologyGrpcService(TechnologyRepository technologyRepository) {
        this.technologyRepository = technologyRepository;
    }

    @Override
    public void getMany(TechnologyProto.GetManyTechnologiesRequest request,
                        StreamObserver<TechnologyProto.TechnologyList> responseObserver) {
        responseObserver.onNext(TechnologyMapper.toProtoList(technologyRepository.findAllByOrderByNameAsc()));
        responseObserver.onCompleted();
    }
}
