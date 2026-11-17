package com.clearshore.data.entity;

import jakarta.persistence.Column;
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

@Entity
@Table(name = "change_request")
public class ChangeRequest {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;

    @Enumerated(EnumType.STRING)
    @Column(name = "content_type")
    private ChangeTarget target;

    @Enumerated(EnumType.STRING)
    private ChangeAction action;

    private Integer targetId;

    @JdbcTypeCode(SqlTypes.JSON)
    @Column(name = "payload")
    private String payloadJson;

    @Enumerated(EnumType.STRING)
    private ChangeStatus status;

    private int requestedBy;

    private Instant createdAt;
    private Instant submittedAt;

    protected ChangeRequest() {
    }

    public ChangeRequest(ChangeTarget target, ChangeAction action, Integer targetId,
                         String payloadJson, ChangeStatus status, int requestedBy) {
        this.target = target;
        this.action = action;
        this.targetId = targetId;
        this.payloadJson = payloadJson;
        this.status = status;
        this.requestedBy = requestedBy;
        this.createdAt = Instant.now();
        this.submittedAt = createdAt;
    }

    public int getId() {
        return id;
    }

    public ChangeTarget getTarget() {
        return target;
    }

    public ChangeAction getAction() {
        return action;
    }

    public Integer getTargetId() {
        return targetId;
    }

    public String getPayloadJson() {
        return payloadJson;
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
}
