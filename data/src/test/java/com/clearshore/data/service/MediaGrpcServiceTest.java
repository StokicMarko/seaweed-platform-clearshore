package com.clearshore.data.service;

import com.clearshore.data.entity.MediaItem;
import com.clearshore.data.entity.MediaType;
import com.clearshore.data.entity.Technology;
import com.clearshore.data.generated.CommonProto;
import com.clearshore.data.generated.MediaProto;
import com.clearshore.data.repository.MediaRepository;
import com.clearshore.data.repository.TechnologyRepository;
import com.clearshore.data.storage.FileStorage;
import com.google.protobuf.Empty;
import io.grpc.Status;
import org.junit.jupiter.api.Test;
import org.springframework.dao.DataIntegrityViolationException;

import java.util.List;
import java.util.Optional;
import java.util.Set;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.verifyNoInteractions;
import static org.mockito.Mockito.when;

class MediaGrpcServiceTest {

  private final MediaRepository media = mock(MediaRepository.class);
  private final TechnologyRepository technologies = mock(TechnologyRepository.class);
  private final FileStorage files = mock(FileStorage.class);
  private final MediaGrpcService service = new MediaGrpcService(media, technologies, files);

  private static MediaItem item() {
    return new MediaItem(Set.of(new Technology("mesh-boom-hd")), MediaType.VIDEO,
        "Boom", "Barrera", "Barrage",
        "Desc", "Desc es", "Desc fr",
        "boom.mp4", "aaa-boom.mp4", "video/mp4");
  }

  private static CommonProto.LocalizedText text(String value) {
    return CommonProto.LocalizedText.newBuilder().setEn(value).setEs(value).setFr(value).build();
  }

  private static MediaProto.CreateMediaRequest.Builder createRequest() {
    return MediaProto.CreateMediaRequest.newBuilder()
        .addTechnologyTypes("mesh-boom-hd")
        .setMediaType(MediaProto.MediaType.MEDIA_TYPE_VIDEO)
        .setTitle(text("Boom"))
        .setOriginalFileName("boom.mp4")
        .setStoredFileName("aaa-boom.mp4")
        .setContentType("video/mp4");
  }

  private static MediaProto.UpdateMediaRequest.Builder updateRequest() {
    return MediaProto.UpdateMediaRequest.newBuilder()
        .setId(1)
        .addTechnologyTypes("mesh-boom-hd")
        .setTitle(text("New"));
  }

  private void technologyExists() {
    when(technologies.findAllById(any())).thenReturn(List.of(new Technology("mesh-boom-hd")));
  }

  // ---- create ----

  @Test
  void createReturnsTheExistingItemAndCreatesNothing() {
    when(media.findByStoredFileName("aaa-boom.mp4")).thenReturn(Optional.of(item()));
    Recorder<MediaProto.Media> out = new Recorder<>();

    service.create(createRequest().build(), out);

    assertNull(out.error);
    assertEquals("aaa-boom.mp4", out.value.getStoredFileName());
    verify(media, never()).save(any());
  }

  @Test
  void createSavesANewItem() {
    when(media.findByStoredFileName("aaa-boom.mp4")).thenReturn(Optional.empty());
    technologyExists();
    when(media.save(any())).thenAnswer(i -> i.getArgument(0));
    Recorder<MediaProto.Media> out = new Recorder<>();

    service.create(createRequest().build(), out);

    assertNull(out.error);
    assertEquals("Boom", out.value.getTitle().getEn());
    assertEquals("aaa-boom.mp4", out.value.getStoredFileName());
    assertFalse(out.value.hasDescription());
    verify(media).save(any());
  }

  @Test
  void createRejectsAnUnknownTechnology() {
    when(media.findByStoredFileName("aaa-boom.mp4")).thenReturn(Optional.empty());
    when(technologies.findAllById(any())).thenReturn(List.of());
    Recorder<MediaProto.Media> out = new Recorder<>();

    service.create(createRequest().build(), out);

    assertEquals(Status.Code.INVALID_ARGUMENT, out.code());
    verify(media, never()).save(any());
  }

  @Test
  void createReturnsTheWinnerWhenTwoIdenticalCreatesRace() {
    when(media.findByStoredFileName("aaa-boom.mp4"))
        .thenReturn(Optional.empty(), Optional.of(item()));
    technologyExists();
    when(media.save(any())).thenThrow(new DataIntegrityViolationException("duplicate stored_file_name"));
    Recorder<MediaProto.Media> out = new Recorder<>();

    service.create(createRequest().build(), out);

    assertNull(out.error);
    assertEquals("aaa-boom.mp4", out.value.getStoredFileName());
  }

  // ---- update ----

  @Test
  void updateKeepsTheFileWhenTheFileFieldsAreNotSet() {
    when(media.findById(1)).thenReturn(Optional.of(item()));
    technologyExists();
    when(media.saveAndFlush(any())).thenAnswer(i -> i.getArgument(0));
    Recorder<MediaProto.Media> out = new Recorder<>();

    service.update(updateRequest().build(), out);

    assertNull(out.error);
    assertEquals("New", out.value.getTitle().getEn());
    assertEquals("aaa-boom.mp4", out.value.getStoredFileName());
    assertEquals("boom.mp4", out.value.getOriginalFileName());
    assertEquals(MediaProto.MediaType.MEDIA_TYPE_VIDEO, out.value.getMediaType());
  }

  @Test
  void updateWithoutDescriptionRemovesTheDescription() {
    when(media.findById(1)).thenReturn(Optional.of(item()));
    technologyExists();
    when(media.saveAndFlush(any())).thenAnswer(i -> i.getArgument(0));
    Recorder<MediaProto.Media> out = new Recorder<>();

    service.update(updateRequest().build(), out);

    assertFalse(out.value.hasDescription());
  }

  @Test
  void updateReplacesTheFileWhenAllFourFileFieldsAreSet() {
    when(media.findById(1)).thenReturn(Optional.of(item()));
    technologyExists();
    when(media.saveAndFlush(any())).thenAnswer(i -> i.getArgument(0));
    Recorder<MediaProto.Media> out = new Recorder<>();

    service.update(updateRequest()
        .setMediaType(MediaProto.MediaType.MEDIA_TYPE_IMAGE)
        .setOriginalFileName("a.png")
        .setStoredFileName("bbb-a.png")
        .setContentType("image/png")
        .build(), out);

    assertNull(out.error);
    assertEquals("bbb-a.png", out.value.getStoredFileName());
    assertEquals("a.png", out.value.getOriginalFileName());
    assertEquals("image/png", out.value.getContentType());
    assertEquals(MediaProto.MediaType.MEDIA_TYPE_IMAGE, out.value.getMediaType());
  }

  @Test
  void updateRejectsSomeButNotAllFileFields() {
    Recorder<MediaProto.Media> out = new Recorder<>();

    service.update(updateRequest().setStoredFileName("bbb-a.png").build(), out);

    assertEquals(Status.Code.INVALID_ARGUMENT, out.code());
    verify(media, never()).saveAndFlush(any());
  }

  @Test
  void updateNotFound() {
    technologyExists();
    when(media.findById(1)).thenReturn(Optional.empty());
    Recorder<MediaProto.Media> out = new Recorder<>();

    service.update(updateRequest().build(), out);

    assertEquals(Status.Code.NOT_FOUND, out.code());
  }

  // ---- delete ----

  @Test
  void deleteRemovesTheRowAndNeverTouchesFiles() {
    MediaItem existing = item();
    when(media.findById(1)).thenReturn(Optional.of(existing));
    Recorder<Empty> out = new Recorder<>();

    service.delete(MediaProto.MediaId.newBuilder().setId(1).build(), out);

    verify(media).delete(existing);
    verifyNoInteractions(files);
    assertTrue(out.completed);
  }

  @Test
  void deleteNotFound() {
    when(media.findById(1)).thenReturn(Optional.empty());
    Recorder<Empty> out = new Recorder<>();

    service.delete(MediaProto.MediaId.newBuilder().setId(1).build(), out);

    assertEquals(Status.Code.NOT_FOUND, out.code());
    verify(media, never()).delete(any());
  }
}