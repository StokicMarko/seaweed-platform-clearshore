package com.clearshore.data.entity;

import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import org.hibernate.annotations.JdbcTypeCode;
import org.hibernate.type.SqlTypes;

import java.time.Instant;


// Each ChangeRequest object = one change a staff member wants to make.
@Entity
@Table(name = "change_request")
public class ChangeRequest {


    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;

    // WHAT the request is about
    @Enumerated(EnumType.STRING)
    private ChangeTarget targetType;

    // WHAT is done to it: create, update or delete.
    @Enumerated(EnumType.STRING)
    private ChangeType type;

    // Id of the existing item for an update or delete.
    private Integer targetId;


    // Payload = The column is jsonb, so this annotation tells Hibernate to treat it as JSON.
    // Without it, ddl-auto=validate would fail on the type apparently :(
    @JdbcTypeCode(SqlTypes.JSON)
    private String payload;

    // PENDING (waiting for the Main Admin) or RETURNED (sent back to staff to fix).
    @Enumerated(EnumType.STRING)
    private ChangeStatus status;

    // Who submitted it.
    // A plain number for now (UPDATE NEXT SPRINT PLSSS)
    private int requestedBy;

    // When the request was made. filters main admin (talk gorup)
    private Instant createdAt;

    // Who reviewed it. Integer because it is empty until someone reviews it.
    private Integer reviewedBy;

    // Hibernate needs an empty constructor to build objects when it reads rows.
    protected ChangeRequest() {
    }

    // creates a new request, request always starts PENDING and can never start with a wrong status.

    public ChangeRequest(ChangeTarget targetType, ChangeType type,
                         Integer targetId, String payload, int requestedBy) {
        this.targetType = targetType;
        this.type = type;
        this.targetId = targetId;
        this.payload = payload;
        this.requestedBy = requestedBy;
        this.status = ChangeStatus.PENDING;
        this.createdAt = Instant.now();
    }



    public int getId() {
        return id;
    }

    public ChangeTarget getTargetType() {
        return targetType;
    }

    public ChangeType getType() {
        return type;
    }

    public Integer getTargetId() {
        return targetId;
    }

    public String getPayload() {
        return payload;
    }

    public ChangeStatus getStatus() {
        return status;
    }

    public int getRequestedBy() {
        return requestedBy;
    }

    public Instant getCreatedAt() {
        return createdAt;
    }

    public Integer getReviewedBy() {
        return reviewedBy;
    }
}