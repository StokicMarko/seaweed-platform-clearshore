package com.clearshore.data.storage;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;

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
}
