package com.clearshore.data.service;

import com.clearshore.data.entity.Media;
import com.clearshore.data.repository.MediaRepository;
import com.clearshore.data.storage.FileStorage;
import org.springframework.stereotype.Service;

import java.io.InputStream;
import java.io.IOException;
import java.util.List;
import java.util.Optional;

@Service
public class MediaService {

  private final MediaRepository mediaRepository;
  private final FileStorage fileStorage;

  public MediaService(MediaRepository mediaRepository, FileStorage fileStorage) {
    this.mediaRepository = mediaRepository;
    this.fileStorage = fileStorage;
  }

  public List<Media> getMany(String machineCode) {
    return machineCode != null && !machineCode.isEmpty()
        ? mediaRepository.findByMachineCodeOrderByPublishedAtDesc(machineCode)
        : mediaRepository.findAllByOrderByPublishedAtDesc();
  }

  public Optional<Media> getSingle(int id) {
    return mediaRepository.findById(id);
  }

  public Media saveMedia(Media media) {
    return mediaRepository.save(media);
  }

  public void deleteMedia(int id) {
    mediaRepository.deleteById(id);
  }

  public InputStream getFileStream(String storedFileName) throws IOException {
    return fileStorage.open(storedFileName);
  }
}