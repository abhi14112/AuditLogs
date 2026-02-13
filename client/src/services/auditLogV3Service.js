import api from '../lib/api';

const BASE_URL = '/auditlogsv3';

/**
 * AuditLogV3 Service - Enterprise-level Jira-style audit logging API client
 */
export const auditLogV3Service = {
  /**
   * Get paginated audit logs with filtering and sorting
   */
  async getAuditLogs({
    page = 1,
    pageSize = 20,
    searchTerm = null,
    entityType = null,
    action = null,
    severity = null,
    userId = null,
    startDate = null,
    endDate = null,
    sortBy = 'createdAt',
    sortDescending = true
  } = {}) {
    const params = new URLSearchParams();
    
    params.append('page', page);
    params.append('pageSize', pageSize);
    if (searchTerm) params.append('searchTerm', searchTerm);
    if (entityType) params.append('entityType', entityType);
    if (action) params.append('action', action);
    if (severity) params.append('severity', severity);
    if (userId) params.append('userId', userId);
    if (startDate) params.append('startDate', startDate.toISOString());
    if (endDate) params.append('endDate', endDate.toISOString());
    if (sortBy) params.append('sortBy', sortBy);
    params.append('sortDescending', sortDescending);

    const response = await api.get(`${BASE_URL}?${params.toString()}`);
    return response.data;
  },

  /**
   * Get audit log by ID
   */
  async getAuditLogById(id) {
    const response = await api.get(`${BASE_URL}/${id}`);
    return response.data;
  },

  /**
   * Get audit logs for a specific entity
   */
  async getAuditLogsByEntity(entityId, entityType = null) {
    const params = new URLSearchParams();
    if (entityType) params.append('entityType', entityType);
    
    const response = await api.get(`${BASE_URL}/entity/${entityId}?${params.toString()}`);
    return response.data;
  },

  /**
   * Get audit logs for a specific user
   */
  async getAuditLogsByUser(userId, top = 100) {
    const response = await api.get(`${BASE_URL}/user/${userId}?top=${top}`);
    return response.data;
  },

  /**
   * Get audit logs by correlation ID
   */
  async getAuditLogsByCorrelation(correlationId) {
    const response = await api.get(`${BASE_URL}/correlation/${correlationId}`);
    return response.data;
  },

  /**
   * Get audit logs by severity
   */
  async getAuditLogsBySeverity(severity) {
    const response = await api.get(`${BASE_URL}/severity/${severity}`);
    return response.data;
  },

  /**
   * Create a new audit log entry
   */
  async createAuditLog(auditLogData) {
    const response = await api.post(BASE_URL, auditLogData);
    return response.data;
  },

  /**
   * Get current user's audit logs
   */
  async getMyAuditLogs(top = 100) {
    const response = await api.get(`${BASE_URL}/my-logs?top=${top}`);
    return response.data;
  },

  /**
   * Get audit log statistics
   */
  async getAuditLogStats(startDate = null, endDate = null) {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate.toISOString());
    if (endDate) params.append('endDate', endDate.toISOString());
    
    const response = await api.get(`${BASE_URL}/stats?${params.toString()}`);
    return response.data;
  }
};

export default auditLogV3Service;
