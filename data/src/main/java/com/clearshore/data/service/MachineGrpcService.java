package com.clearshore.data.service;

import com.clearshore.data.generated.MachineDataGrpc;
import com.clearshore.data.generated.MachineProto;
import com.clearshore.data.mapper.MachineMapper;
import com.clearshore.data.repository.MachineRepository;
import io.grpc.stub.StreamObserver;
import org.springframework.grpc.server.service.GrpcService;

@GrpcService
public class MachineGrpcService extends MachineDataGrpc.MachineDataImplBase {

    private final MachineRepository machineRepository;

    public MachineGrpcService(MachineRepository machineRepository) {
        this.machineRepository = machineRepository;
    }

    @Override
    public void getMany(MachineProto.GetManyMachinesRequest request,
                        StreamObserver<MachineProto.MachineList> responseObserver) {
        responseObserver.onNext(MachineMapper.toProtoList(machineRepository.findAllByOrderByNameAsc()));
        responseObserver.onCompleted();
    }
}
