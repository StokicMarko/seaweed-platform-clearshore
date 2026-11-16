package com.clearshore.data.mapper;

import com.clearshore.data.entity.Machine;
import com.clearshore.data.generated.MachineProto;

import java.util.List;

public final class MachineMapper {

    private MachineMapper() {
    }

    public static MachineProto.Machine toProto(Machine machine) {
        return MachineProto.Machine.newBuilder()
                .setId(machine.getId())
                .setCode(machine.getCode())
                .setName(machine.getName())
                .build();
    }

    public static MachineProto.MachineList toProtoList(List<Machine> machines) {
        return MachineProto.MachineList.newBuilder()
                .addAllItems(machines.stream().map(MachineMapper::toProto).toList())
                .build();
    }
}
