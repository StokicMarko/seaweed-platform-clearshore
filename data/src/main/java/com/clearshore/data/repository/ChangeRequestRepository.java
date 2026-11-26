package com.clearshore.data.repository;

import com.clearshore.data.entity.ChangeRequest;
import com.clearshore.data.entity.ChangeStatus;
import com.clearshore.data.entity.ChangeTarget;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.List;

// Gives us save, findById, delete and so on for change requests, with no code (jpa repo includes it)
// findFiltered is the one custom query: a filter that is null is not applied.
public interface ChangeRequestRepository extends JpaRepository<ChangeRequest, Integer> {

  @Query("""
           select c from ChangeRequest c
           where (:targetType is null or c.targetType = :targetType)
             and (:targetId is null or c.targetId = :targetId)
             and (:status is null or c.status = :status)
           order by c.createdAt desc
           """)
  List<ChangeRequest> findFiltered(@Param("targetType") ChangeTarget targetType,
      @Param("targetId") Integer targetId,
      @Param("status") ChangeStatus status);
}