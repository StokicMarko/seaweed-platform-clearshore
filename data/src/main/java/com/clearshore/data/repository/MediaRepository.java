package com.clearshore.data.repository;

import com.clearshore.data.entity.Media;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

@Repository
public interface MediaRepository extends JpaRepository<Media, Integer> {

    List<Media> findAllByOrderByPublishedAtDesc();

    List<Media> findByMachineCodeOrderByPublishedAtDesc(String machineCode);
}
