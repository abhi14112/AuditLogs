import * as signalR from '@microsoft/signalr';

const HUB_URL = 'https://localhost:7237/hubs/auditlogsv3';

/**
 * SignalR Service for real-time AuditLogV3 updates
 */
class SignalRV3Service {
  constructor() {
    this.connection = null;
    this.isConnected = false;
    this.listeners = new Map();
  }

  /**
   * Initialize and start SignalR connection
   */
  async start() {
    if (this.connection && this.isConnected) {
      console.log('SignalR V3: Already connected');
      return;
    }

    try {
      // Get JWT token from localStorage
      const token = localStorage.getItem('token');

      // Create connection
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(HUB_URL, {
          accessTokenFactory: () => token || '',
          skipNegotiation: false,
          withCredentials: true
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext) => {
            // Exponential backoff: 0, 2, 10, 30 seconds, then 30 seconds
            if (retryContext.previousRetryCount === 0) return 0;
            if (retryContext.previousRetryCount === 1) return 2000;
            if (retryContext.previousRetryCount === 2) return 10000;
            return 30000;
          }
        })
        .configureLogging(signalR.LogLevel.Information)
        .build();

      // Set up event handlers
      this.connection.onclose((error) => {
        this.isConnected = false;
        console.log('SignalR V3: Connection closed', error);
        this.notifyListeners('connectionClosed', error);
      });

      this.connection.onreconnecting((error) => {
        this.isConnected = false;
        console.log('SignalR V3: Reconnecting...', error);
        this.notifyListeners('reconnecting', error);
      });

      this.connection.onreconnected((connectionId) => {
        this.isConnected = true;
        console.log('SignalR V3: Reconnected', connectionId);
        this.notifyListeners('reconnected', connectionId);
      });

      // Listen for audit log updates
      this.connection.on('ReceiveAuditLogV3', (auditLog) => {
        this.notifyListeners('auditLogReceived', auditLog);
      });

      this.connection.on('Connected', (connectionId) => {
        console.log('SignalR V3: Connected with ID:', connectionId);
        this.notifyListeners('connected', connectionId);
      });

      this.connection.on('JoinedRoom', (roomName) => {
        console.log('SignalR V3: Joined room:', roomName);
        this.notifyListeners('joinedRoom', roomName);
      });

      this.connection.on('LeftRoom', (roomName) => {
        console.log('SignalR V3: Left room:', roomName);
        this.notifyListeners('leftRoom', roomName);
      });

      // Start connection
      await this.connection.start();
      this.isConnected = true;
      console.log('SignalR V3: Connection established');
      
    } catch (error) {
      console.error('SignalR V3: Connection error:', error);
      this.isConnected = false;
      throw error;
    }
  }

  /**
   * Stop SignalR connection
   */
  async stop() {
    if (this.connection) {
      try {
        await this.connection.stop();
        this.isConnected = false;
        console.log('SignalR V3: Connection stopped');
      } catch (error) {
        console.error('SignalR V3: Error stopping connection:', error);
      }
    }
  }

  /**
   * Subscribe to events
   */
  on(eventName, callback) {
    if (!this.listeners.has(eventName)) {
      this.listeners.set(eventName, []);
    }
    this.listeners.get(eventName).push(callback);

    // Return unsubscribe function
    return () => this.off(eventName, callback);
  }

  /**
   * Unsubscribe from events
   */
  off(eventName, callback) {
    if (this.listeners.has(eventName)) {
      const callbacks = this.listeners.get(eventName);
      const index = callbacks.indexOf(callback);
      if (index > -1) {
        callbacks.splice(index, 1);
      }
    }
  }

  /**
   * Notify all listeners for an event
   */
  notifyListeners(eventName, data) {
    if (this.listeners.has(eventName)) {
      this.listeners.get(eventName).forEach(callback => {
        try {
          callback(data);
        } catch (error) {
          console.error(`Error in ${eventName} listener:`, error);
        }
      });
    }
  }

  /**
   * Join entity room for filtered updates
   */
  async joinEntityRoom(entityId) {
    if (this.connection && this.isConnected) {
      await this.connection.invoke('JoinEntityRoom', entityId);
    }
  }

  /**
   * Leave entity room
   */
  async leaveEntityRoom(entityId) {
    if (this.connection && this.isConnected) {
      await this.connection.invoke('LeaveEntityRoom', entityId);
    }
  }

  /**
   * Join user room for filtered updates
   */
  async joinUserRoom(userId) {
    if (this.connection && this.isConnected) {
      await this.connection.invoke('JoinUserRoom', userId);
    }
  }

  /**
   * Leave user room
   */
  async leaveUserRoom(userId) {
    if (this.connection && this.isConnected) {
      await this.connection.invoke('LeaveUserRoom', userId);
    }
  }

  /**
   * Get connection state
   */
  getConnectionState() {
    if (!this.connection) return 'Disconnected';
    
    switch (this.connection.state) {
      case signalR.HubConnectionState.Connected:
        return 'Connected';
      case signalR.HubConnectionState.Connecting:
        return 'Connecting';
      case signalR.HubConnectionState.Reconnecting:
        return 'Reconnecting';
      case signalR.HubConnectionState.Disconnected:
        return 'Disconnected';
      default:
        return 'Unknown';
    }
  }

  /**
   * Check if connected
   */
  isConnectionActive() {
    return this.isConnected && this.connection?.state === signalR.HubConnectionState.Connected;
  }
}

// Create singleton instance
const signalRV3Service = new SignalRV3Service();

export default signalRV3Service;
