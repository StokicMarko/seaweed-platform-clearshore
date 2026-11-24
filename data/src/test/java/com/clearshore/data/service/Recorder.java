package com.clearshore.data.service;

import io.grpc.Status;
import io.grpc.StatusRuntimeException;
import io.grpc.stub.StreamObserver;

class Recorder<T> implements StreamObserver<T> {
  T value;
  Throwable error;
  boolean completed;

  @Override public void onNext(T v) { value = v; }
  @Override public void onError(Throwable t) { error = t; }
  @Override public void onCompleted() { completed = true; }

  Status.Code code() {
    return ((StatusRuntimeException) error).getStatus().getCode();
  }
}