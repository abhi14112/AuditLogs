import axios from 'axios';

const API_BASE_URL = 'https://localhost:7237/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add auth token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle 401 errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Helper function to extract error message from API response
export const getErrorMessage = (error) => {
  // Network error (no response from server)
  if (!error.response) {
    if (error.message === 'Network Error') {
      return 'Unable to connect to the server. Please check if the backend is running.';
    }
    return error.message || 'An unexpected error occurred';
  }

  // Server responded with error
  const { status, data } = error.response;
  
  // Extract message from various possible response formats
  if (data?.message) {
    return data.message;
  }
  
  if (typeof data === 'string') {
    return data;
  }

  // Default messages based on status code
  switch (status) {
    case 400:
      return 'Invalid request. Please check your input.';
    case 401:
      return 'Unauthorized. Please log in again.';
    case 403:
      return 'You do not have permission to perform this action.';
    case 404:
      return 'The requested resource was not found.';
    case 500:
      return 'Internal server error. Please try again later.';
    default:
      return `Request failed with status ${status}`;
  }
};

export default api;
