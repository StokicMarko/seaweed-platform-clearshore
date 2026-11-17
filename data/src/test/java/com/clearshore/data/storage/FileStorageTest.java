package com.clearshore.data.storage;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.file.Files;
import java.nio.file.NoSuchFileException;
import java.nio.file.Path;

import static org.junit.jupiter.api.Assertions.assertArrayEquals;
import static org.junit.jupiter.api.Assertions.assertDoesNotThrow;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class FileStorageTest {

    @TempDir
    Path folder;

    private FileStorage storage;

    @BeforeEach
    void setUp() throws IOException {
        storage = new FileStorage(folder.toString());
    }

    @Test
    void savedBytesCanBeReadBack() throws IOException {
        byte[] content = {1, 2, 3, 4, 5};
        String name = storage.newStoredFileName("boom.mp4");

        try (OutputStream output = storage.create(name)) {
            output.write(content);
        }

        try (InputStream input = storage.open(name)) {
            assertArrayEquals(content, input.readAllBytes());
        }
    }

    @Test
    void storedNameKeepsOnlyTheExtension() {
        String name = storage.newStoredFileName("My Holiday Photo.PNG");

        assertTrue(name.endsWith(".png"));
        assertFalse(name.contains("Holiday"));
    }

    @Test
    void storedNamesAreUnique() {
        assertNotEquals(storage.newStoredFileName("a.png"), storage.newStoredFileName("a.png"));
    }

    @Test
    void storedNameDropsAnyFolderPartOfTheUploadedName() {
        String name = storage.newStoredFileName("../../etc/passwd.txt");

        assertTrue(name.endsWith(".txt"));
        assertFalse(name.contains("/"));
        assertFalse(name.contains(".."));
    }

    @Test
    void deleteRemovesTheFile() throws IOException {
        String name = storage.newStoredFileName("a.png");
        storage.create(name).close();

        storage.delete(name);

        assertThrows(NoSuchFileException.class, () -> storage.open(name));
    }

    @Test
    void deletingAMissingFileIsNotAnError() {
        assertDoesNotThrow(() -> storage.delete("missing.png"));
    }

    @Test
    void namesOutsideTheFolderAreRejected() {
        assertThrows(IllegalArgumentException.class, () -> storage.delete("../x"));
        assertThrows(IllegalArgumentException.class, () -> storage.create("../x"));
    }

    @Test
    void creatingTheSameNameTwiceFails() throws IOException {
        storage.create("a.png").close();

        assertThrows(IOException.class, () -> storage.create("a.png"));
        assertEquals(1, Files.list(folder).count());
    }
}
