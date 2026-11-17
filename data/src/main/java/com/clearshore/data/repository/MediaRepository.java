package com.clearshore.data.repository;

import com.clearshore.data.entity.Media;
import org.springframework.data.jpa.repository.JpaRepository;

import org.springframework.data.jpa.repository.EntityGraph;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.Optional;

import java.util.List;

public interface MediaRepository extends JpaRepository<Media, Integer> {

    @EntityGraph(attributePaths = "technologies")
    List<Media> findAllByOrderByPublishedAtDesc();

    @EntityGraph(attributePaths = "technologies")
    @Query("""
           select m from Media m
           where exists (select 1 from m.technologies t where t.type = :type)
           order by m.publishedAt desc
           """)
    List<Media> findByTechnologyType(@Param("type") String type);

    @Override
    @EntityGraph(attributePaths = "technologies")
    Optional<Media> findById(Integer id);
}
