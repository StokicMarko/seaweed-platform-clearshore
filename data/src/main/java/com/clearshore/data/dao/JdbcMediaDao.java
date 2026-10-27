package com.clearshore.data.dao;

import com.clearshore.data.model.Media;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.core.RowMapper;
import org.springframework.stereotype.Repository;
import java.util.List;
import java.util.Optional;

@Repository
public class JdbcMediaDao implements MediaDao {

    private static final RowMapper<Media> MAPPER = (rs, i) -> new Media(
            rs.getInt("id"),
            rs.getString("title_en"), rs.getString("title_fr"), rs.getString("title_es"),
            rs.getString("description_en"), rs.getString("description_fr"), rs.getString("description_es"),
            rs.getString("file_name"), rs.getString("url"), rs.getString("content_type"),
            rs.getTimestamp("created_at").toInstant(),
            rs.getString("machine"));

    private final JdbcTemplate jdbc;

    public JdbcMediaDao(JdbcTemplate jdbc) {
        this.jdbc = jdbc;
    }

    @Override
    public Media insert(Media m) {
        return jdbc.queryForObject("""
                INSERT INTO media (title_en, title_fr, title_es,
                                   description_en, description_fr, description_es,
                                   file_name, url, content_type, machine)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                RETURNING *""",
                MAPPER,
                m.titleEn(), m.titleFr(), m.titleEs(),
                m.descriptionEn(), m.descriptionFr(), m.descriptionEs(),
                m.fileName(), m.url(), m.contentType(), m.machine());
    }

    @Override
    public Optional<Media> update(Media m) {
        return jdbc.query("""
                UPDATE media SET title_en = ?, title_fr = ?, title_es = ?,
                                 description_en = ?, description_fr = ?, description_es = ?,
                                 machine = ?
                WHERE id = ?
                RETURNING *""",
                MAPPER,
                m.titleEn(), m.titleFr(), m.titleEs(),
                m.descriptionEn(), m.descriptionFr(), m.descriptionEs(),
                m.machine(), m.id()).stream().findFirst();
    }

    @Override
    public List<Media> findAll() {
        return jdbc.query("SELECT * FROM media ORDER BY created_at DESC", MAPPER);
    }

    @Override
    public List<Media> findByMachine(String machine) {
        return jdbc.query("SELECT * FROM media WHERE machine = ? ORDER BY created_at DESC",
                MAPPER, machine);
    }

    @Override
    public Optional<Media> findById(int id) {
        return jdbc.query("SELECT * FROM media WHERE id = ?", MAPPER, id).stream().findFirst();
    }

    @Override
    public boolean delete(int id) {
        return jdbc.update("DELETE FROM media WHERE id = ?", id) > 0;
    }
}

