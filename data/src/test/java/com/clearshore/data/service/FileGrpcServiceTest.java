package com.clearshore.data.service;

import com.clearshore.data.generated.CommonProto;
import com.clearshore.data.generated.FileProto;
import com.clearshore.data.storage.FileStorage;
import com.google.protobuf.ByteString;
import io.grpc.Status;
import io.grpc.StatusRuntimeException;
import io.grpc.stub.StreamObserver;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.nio.file.Path;

import static org.junit.jupiter.api.Assertions.assertArrayEquals;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertNull;

class FileGrpcServiceTest {

    @TempDir
    Path folder;

    private FileStorage storage;
    private FileGrpcService service;

    @BeforeEach
    void setUp() throws IOException {
        storage = new FileStorage(folder.toString());
        service = new FileGrpcService(storage);
    }

    @Test
    void fileReadByNameHasTheSameBytesThatWereSaved() throws IOException {
        byte[] content = new byte[150_000];
        for (int i = 0; i < content.length; i++) {
            content[i] = (byte) i;
        }
        String name = storage.newStoredFileName("boom.mp4");
        try (OutputStream output = storage.create(name)) {
            output.write(content);
        }

        ReadResult result = read(name);

        assertNull(result.error);
        assertArrayEquals(content, result.bytes.toByteArray());
    }

    @Test
    void missingFileGivesNotFound() {
        ReadResult result = read("missing.png");

        assertEquals(Status.Code.NOT_FOUND, result.error.getStatus().getCode());
    }

    @Test
    void nameOutsideTheStorageFolderIsRejected() {
        ReadResult result = read("../x");

        assertEquals(Status.Code.INVALID_ARGUMENT, result.error.getStatus().getCode());
    }

    private ReadResult read(String storedFileName) {
        ReadResult result = new ReadResult();
        service.read(FileProto.StoredFile.newBuilder().setStoredFileName(storedFileName).build(), result);
        return result;
    }

    private static class ReadResult implements StreamObserver<CommonProto.FileChunk> {
        final ByteArrayOutputStream bytes = new ByteArrayOutputStream();
        StatusRuntimeException error;
        boolean completed;

        @Override
        public void onNext(CommonProto.FileChunk chunk) {
            ByteString data = chunk.getData();
            assertNotNull(data);
            bytes.writeBytes(data.toByteArray());
        }

        @Override
        public void onError(Throwable t) {
            error = (StatusRuntimeException) t;
        }

        @Override
        public void onCompleted() {
            completed = true;
        }
    }
}
