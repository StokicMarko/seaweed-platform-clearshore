package com.clearshore.data.entity;

import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import java.util.Objects;

@Entity
@Table(name = "technology")
public class Technology {

    @Id
    private String type;

    protected Technology() {
    }

    public Technology(String type) {
        this.type=type;
    }


    public String getType() {
        return type;
    }

    @Override
    public boolean equals(Object o) {
        return o instanceof Technology t && Objects.equals(type, t.getType());
    }
    @Override
    public int hashCode() { return Objects.hashCode(type); }
}
