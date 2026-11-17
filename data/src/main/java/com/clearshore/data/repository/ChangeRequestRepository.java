package com.clearshore.data.repository;

import com.clearshore.data.entity.ChangeRequest;
import org.springframework.data.jpa.repository.JpaRepository;

public interface ChangeRequestRepository extends JpaRepository<ChangeRequest, Integer> {
}
