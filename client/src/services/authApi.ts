// Types for authentication
export interface RegisterData {
  username: string;
  password: string;
}

export interface LoginData {
  username: string;
  password: string;
}

export interface AuthResponse {
  userId: number;
  username: string;
  token: string;
}

const API_BASE = "http://localhost:5151/api";

// Register new user
export const register = async (data: RegisterData): Promise<AuthResponse> => {
  const response = await fetch(`${API_BASE}/auth/register`, { 
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || "Registration failed");
  }

  return response.json();
};

// Login existing user
export const login = async (data: LoginData): Promise<AuthResponse> => {
  const response = await fetch(`${API_BASE}/auth/login`, { 
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || "Login failed");
  }

  return response.json();
};

// Store token in localStorage
export const saveToken = (token: string) => {
  localStorage.setItem("authToken", token);
};

// Get token from localStorage
export const getToken = (): string | null => {
  return localStorage.getItem("authToken");
};

// Remove token (logout)
export const removeToken = () => {
  localStorage.removeItem("authToken");
};

// Check if user is logged in
export const isAuthenticated = (): boolean => {
  return !!getToken();
};