import axios, { AxiosInstance } from "axios";

const http: AxiosInstance = axios.create({
  baseURL: process.env.VUE_APP_COSTUMES_API_URL,
  headers: {
    "Content-type": "application/json",
  },
});

export default http;
