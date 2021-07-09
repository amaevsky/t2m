export const BASE_URL = "https://localhost:44361";
export const API_BASE_URL = process.env.NODE_ENV === 'development'
  ? 'https://localhost:44361/api'
  : 'https://lingua-api.azurewebsites.net/api'