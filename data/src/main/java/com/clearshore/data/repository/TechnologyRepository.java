package com.clearshore.data.repository;

import com.clearshore.data.entity.Machine;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface TechnologyRepository extends JpaRepository<Technology, String> {

    List<Technology> findAllByOrderByTypeAsc();
}
