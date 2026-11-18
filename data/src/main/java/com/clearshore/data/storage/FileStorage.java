package com.clearshore.data.storage;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;

import java.io.OutputStream;
import java.nio.file.StandardOpenOption;
import java.util.Locale;
import java.util.UUID;

@Component
public class FileStorage {

    private final Path folder;

    public FileStorage(@Value("${media.storage-dir}") String folder) throws IOException {
        this.folder = Files.createDirectories(Path.of(folder).toAbsolutePath().normalize());
    }

    public InputStream open(String storedFileName) throws IOException {
        return Files.newInputStream(resolve(storedFileName));
    }

    private Path resolve(String storedFileName) {
        Path file = folder.resolve(storedFileName).normalize();
        if (!file.startsWith(folder)) {
            throw new IllegalArgumentException("File is outside the media folder: " + storedFileName);
        }
        return file;
    }

    /** A unique name for a new file. Only the extension of the uploaded name is kept. */
    public String newStoredFileName(String originalFileName) {
        String name = Path.of(originalFileName).getFileName().toString();
        int dot = name.lastIndexOf('.');
        String extension = dot >= 0 ? name.substring(dot + 1).toLowerCase(Locale.ROOT) : "";
        return extension.matches("[a-z0-9]{1,10}")
            ? UUID.randomUUID() + "." + extension
            : UUID.randomUUID().toString();
    }

    public OutputStream create(String storedFileName) throws IOException {
        return Files.newOutputStream(resolve(storedFileName), StandardOpenOption.CREATE_NEW);
    }

    public void delete(String storedFileName) throws IOException {
        Files.deleteIfExists(resolve(storedFileName));
    }
}
