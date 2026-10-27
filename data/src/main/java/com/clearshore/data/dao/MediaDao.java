package com.clearshore.data.dao;

import com.clearshore.data.model.Media;
import java.util.List;
import java.util.Optional;

    public interface MediaDao {
        Media insert(Media media);
        Optional<Media> update(Media media);
        List<Media> findAll();
        List<Media> findByMachine(String machine);
        Optional<Media> findById(int id);
        boolean delete(int id);
    }
