package com.clearshore.data.service;

import com.clearshore.data.entity.ChangeRequest;
import com.clearshore.data.entity.ChangeStatus;
import com.clearshore.data.entity.ChangeTarget;
import com.clearshore.data.entity.ChangeType;
import com.clearshore.data.generated.ChangeRequestProto;
import com.clearshore.data.repository.ChangeRequestRepository;
import com.google.protobuf.Empty;
import io.grpc.Status;
import org.junit.jupiter.api.Test;

import java.util.List;
import java.util.Optional;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.never;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

class ChangeRequestGrpcServiceTest {

  private final ChangeRequestRepository repo = mock(ChangeRequestRepository.class);
  private final ChangeRequestGrpcService service = new ChangeRequestGrpcService(repo);

  private static ChangeRequest pending() {
    return new ChangeRequest(ChangeTarget.MEDIA, ChangeType.UPDATE, 5, "{}", 1);
  }

  private static ChangeRequestProto.ChangeRequestId id(int id) {
    return ChangeRequestProto.ChangeRequestId.newBuilder().setId(id).build();
  }

  @Test
  void getManyWithNoFiltersPassesNulls() {
    when(repo.findFiltered(null, null, null)).thenReturn(List.of(pending()));
    Recorder<ChangeRequestProto.ChangeRequestList> out = new Recorder<>();

    service.getMany(ChangeRequestProto.GetManyChangeRequestsRequest.getDefaultInstance(), out);

    assertNull(out.error);
    assertEquals(1, out.value.getItemsCount());
  }

  @Test
  void getManyPassesTheFilters() {
    service.getMany(ChangeRequestProto.GetManyChangeRequestsRequest.newBuilder()
        .setTargetType(ChangeRequestProto.ChangeTarget.CHANGE_TARGET_MEDIA)
        .setTargetId(5)
        .setStatus(ChangeRequestProto.ChangeStatus.CHANGE_STATUS_PENDING)
        .build(), new Recorder<>());

    verify(repo).findFiltered(ChangeTarget.MEDIA, 5, ChangeStatus.PENDING);
  }

  @Test
  void getSingleReturnsTheRequest() {
    when(repo.findById(1)).thenReturn(Optional.of(pending()));
    Recorder<ChangeRequestProto.ChangeRequest> out = new Recorder<>();

    service.getSingle(id(1), out);

    assertEquals(ChangeRequestProto.ChangeTarget.CHANGE_TARGET_MEDIA, out.value.getTargetType());
    assertEquals(5, out.value.getTargetId());
  }

  @Test
  void getSingleNotFound() {
    when(repo.findById(9)).thenReturn(Optional.empty());
    Recorder<ChangeRequestProto.ChangeRequest> out = new Recorder<>();

    service.getSingle(id(9), out);

    assertEquals(Status.Code.NOT_FOUND, out.code());
  }

  @Test
  void updateStatusSavesTheNewStatus() {
    when(repo.findById(1)).thenReturn(Optional.of(pending()));
    when(repo.save(any())).thenAnswer(i -> i.getArgument(0));
    Recorder<ChangeRequestProto.ChangeRequest> out = new Recorder<>();

    service.updateStatus(ChangeRequestProto.UpdateChangeRequestStatusRequest.newBuilder()
        .setId(1)
        .setStatus(ChangeRequestProto.ChangeStatus.CHANGE_STATUS_RETURNED)
        .build(), out);

    assertNull(out.error);
    assertEquals(ChangeRequestProto.ChangeStatus.CHANGE_STATUS_RETURNED, out.value.getStatus());
  }

  @Test
  void updateStatusRejectsAnUnspecifiedStatus() {
    Recorder<ChangeRequestProto.ChangeRequest> out = new Recorder<>();

    service.updateStatus(ChangeRequestProto.UpdateChangeRequestStatusRequest.newBuilder()
        .setId(1).build(), out);

    assertEquals(Status.Code.INVALID_ARGUMENT, out.code());
    verify(repo, never()).save(any());
  }

  @Test
  void updateStatusNotFound() {
    when(repo.findById(1)).thenReturn(Optional.empty());
    Recorder<ChangeRequestProto.ChangeRequest> out = new Recorder<>();

    service.updateStatus(ChangeRequestProto.UpdateChangeRequestStatusRequest.newBuilder()
        .setId(1)
        .setStatus(ChangeRequestProto.ChangeStatus.CHANGE_STATUS_RETURNED)
        .build(), out);

    assertEquals(Status.Code.NOT_FOUND, out.code());
  }

  @Test
  void deleteRemovesTheRequest() {
    when(repo.existsById(1)).thenReturn(true);
    Recorder<Empty> out = new Recorder<>();

    service.delete(id(1), out);

    verify(repo).deleteById(1);
    assertTrue(out.completed);
  }

  @Test
  void deleteNotFound() {
    when(repo.existsById(1)).thenReturn(false);
    Recorder<Empty> out = new Recorder<>();

    service.delete(id(1), out);

    assertEquals(Status.Code.NOT_FOUND, out.code());
    verify(repo, never()).deleteById(any());
  }
}