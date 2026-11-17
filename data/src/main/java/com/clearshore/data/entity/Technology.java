package com.clearshore.data.entity;

import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Table;

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
}
