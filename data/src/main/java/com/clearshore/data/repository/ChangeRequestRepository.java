package com.clearshore.data.repository;

import com.clearshore.data.entity.ChangeRequest;
import org.springframework.data.jpa.repository.JpaRepository;

// Gives us save, findById, delete and so on for change requests, with no code (jpa repo includes it)
public interface ChangeRequestRepository extends JpaRepository<ChangeRequest, Integer> {
}