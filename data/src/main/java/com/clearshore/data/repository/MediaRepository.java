package com.clearshore.data.repository;

import com.clearshore.data.entity.MediaItem;
import org.springframework.data.jpa.repository.JpaRepository;

import org.springframework.data.jpa.repository.EntityGraph;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.util.Optional;

import java.util.List;

public interface MediaRepository extends JpaRepository<MediaItem, Integer> {

    @EntityGraph(attributePaths = "technologies")
    List<MediaItem> findAllByOrderByPublishedAtDesc();

    @EntityGraph(attributePaths = "technologies")
    @Query("""
           select m from MediaItem m
           where exists (select 1 from m.technologies t where t.type = :type)
           order by m.publishedAt desc
           """)
    List<MediaItem> findByTechnologyType(@Param("type") String type);

    @Override
    @EntityGraph(attributePaths = "technologies")
    Optional<MediaItem> findById(Integer id);

    @EntityGraph(attributePaths = "technologies")
    @Query("select m from MediaItem m where m.file.storedFileName = :storedFileName")
    Optional<MediaItem> findByStoredFileName(@Param("storedFileName") String storedFileName);
}
