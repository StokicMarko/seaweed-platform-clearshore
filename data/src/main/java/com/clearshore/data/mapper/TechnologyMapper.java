package com.clearshore.data.mapper;

import com.clearshore.data.entity.Technology;
import com.clearshore.data.generated.TechnologyProto;

import java.util.List;

public final class TechnologyMapper {

    private TechnologyMapper() {
    }

    public static TechnologyProto.Technology toProto(Technology technology) {
        return TechnologyProto.Technology.newBuilder()
                .setType(technology.getType())
                .build();
    }

    public static TechnologyProto.TechnologyList toProtoList(List<Technology> technologies) {
        return TechnologyProto.TechnologyList.newBuilder()
                .addAllItems(technologies.stream().map(TechnologyMapper::toProto).toList())
                .build();
    }
}
