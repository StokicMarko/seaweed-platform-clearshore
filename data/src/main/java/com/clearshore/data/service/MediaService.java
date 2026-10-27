package com.clearshore.data.service;

import com.clearshore.data.dao.MediaDao;
import com.clearshore.data.model.Media;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;
import java.util.NoSuchElementException;
import java.util.UUID;

@Service
public class MediaService {

    private static final String URL_PREFIX = "/media/";

    private final MediaDao dao;
    private final Path storageDir;

    public MediaService(MediaDao dao, @Value("${media.storage-dir:media-files}") String dir)
            throws IOException {
        this.dao = dao;
        this.storageDir = Files.createDirectories(Path.of(dir));
    }

    public List<Media> list(String machine) {
        return machine == null || machine.isBlank() ? dao.findAll() : dao.findByMachine(machine);
    }

    public Media get(int id) {
        return dao.findById(id)
                .orElseThrow(() -> new NoSuchElementException("Media " + id + " not found"));
    }

    public Media update(int id, Media changes) {
        requireFields(changes);
        return dao.update(changes.withId(id))
                .orElseThrow(() -> new NoSuchElementException("Media " + id + " not found"));
    }

    public boolean delete(int id) {
        Media media = get(id);
        boolean deleted = dao.delete(id);
        if (deleted) {
            deleteFile(media.url());
        }
        return deleted;
    }

    /** Called when the first (metadata) message arrives: returns where the file will be written. */
    public Path prepareFile(String fileName) {
        if (fileName == null || fileName.isBlank()) {
            throw new IllegalArgumentException("file_name is required");
        }
        String safe = fileName.replaceAll("[^A-Za-z0-9._-]", "_");
        return storageDir.resolve(UUID.randomUUID() + "-" + safe);
    }

    /** Called when the stream is complete: validates and inserts the row. */
    public Media create(Media draft, Path file) {
        try {
            requireFields(draft);
            return dao.insert(draft.withUrl(URL_PREFIX + file.getFileName()));
        } catch (RuntimeException e) {
            discard(file);
            throw e;
        }
    }

    public void discard(Path file) {
        try {
            if (file != null) Files.deleteIfExists(file);
        } catch (IOException ignored) {
        }
    }

    private void deleteFile(String url) {
        if (url != null && url.startsWith(URL_PREFIX)) {
            discard(storageDir.resolve(url.substring(URL_PREFIX.length())));
        }
    }

    private static void requireFields(Media m) {
        if (isBlank(m.titleEn()) || isBlank(m.titleFr()) || isBlank(m.titleEs())) {
            throw new IllegalArgumentException("Titles in English, French and Spanish are required");
        }
        if (isBlank(m.machine())) {
            throw new IllegalArgumentException("A machine is required");
        }
    }

    private static boolean isBlank(String s) {
        return s == null || s.isBlank();
    }
}

/*@Service: Spring creates one instance and injects it into the controller.
Constructor: takes the MediaDao interface, so the service doesn’t know about JDBC. @Value reads media.storage-dir (default media-files). It creates the folder at startup.
list: empty machine means all, otherwise filter by machine.
        get / update: not found throws NoSuchElementException, which the controller maps to gRPC NOT_FOUND.
        delete: reads the item, deletes the DB row, then deletes the file. A leftover file is harmless, but a row pointing at a missing file is a visible bug.
prepareFile: the regex keeps only letters, digits, ., - and _, so ../../x can’t escape the folder. The UUID prefix stops overwrites.
        create: validate, set the url, insert. On failure it deletes the file.
requireFields: the business rules (three titles, a machine). The DB NOT NULL is the safety net./
*/
