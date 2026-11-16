package com.clearshore.data.mapper;

import com.clearshore.data.generated.CommonProto;
import com.google.protobuf.Timestamp;

import java.time.Instant;

public final class CommonMapper {

    private CommonMapper() {
    }

    public static CommonProto.LocalizedText toLocalizedText(String en, String es, String fr) {
        return CommonProto.LocalizedText.newBuilder()
                .setEn(en)
                .setEs(es)
                .setFr(fr)
                .build();
    }

    public static Timestamp toTimestamp(Instant instant) {
        return Timestamp.newBuilder()
                .setSeconds(instant.getEpochSecond())
                .setNanos(instant.getNano())
                .build();
    }
}
