package com.clearshore.data.storage;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.charset.StandardCharsets;
import java.nio.file.Path;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNotEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class FileStorageTest {

  @TempDir
  Path folder;

  @Test
  void savedFileCanBeReadBackAndDeleted() throws IOException {
    FileStorage storage = new FileStorage(folder.toString());
    String name = storage.newStoredFileName("boom.mp4");

    try (OutputStream out = storage.create(name)) {
      out.write("hello".getBytes(StandardCharsets.UTF_8));
    }
    try (InputStream in = storage.open(name)) {
      assertEquals("hello", new String(in.readAllBytes(), StandardCharsets.UTF_8));
    }

    storage.delete(name);
    assertFalse(java.nio.file.Files.exists(folder.resolve(name)));
    storage.delete(name); // deleting twice is fine
  }

  @Test
  void storedNamesAreUniqueAndKeepOnlyASafeExtension() throws IOException {
    FileStorage storage = new FileStorage(folder.toString());

    String first = storage.newStoredFileName("Photo.JPG");
    String second = storage.newStoredFileName("Photo.JPG");

    assertNotEquals(first, second);
    assertTrue(first.endsWith(".jpg"));
    assertFalse(storage.newStoredFileName("../../evil").contains("/"));
    assertFalse(storage.newStoredFileName("noextension").contains("."));
  }

  @Test
  void refusesToLeaveTheMediaFolder() throws IOException {
    FileStorage storage = new FileStorage(folder.toString());

    assertThrows(IllegalArgumentException.class, () -> storage.open("../secret.txt"));
    assertThrows(IllegalArgumentException.class, () -> storage.delete("../secret.txt"));
  }
}