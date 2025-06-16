import axios from "axios";
import { useUserStore } from "../stores/userStore";

const API_BASE_URL = import.meta.env.VITE_API_URL_PROD;
// const API_BASE_URL = import.meta.env.VITE_API_URL_LOCAL;

const apiClient = axios.create({
  baseURL: API_BASE_URL,
})

apiClient.interceptors.request.use(config => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default apiClient;

// ======= AUTH =======

export const login = async (credentials) => {
  try {
    const response = await apiClient.post("/Auth/login", credentials);
    const {token, id} = response.data;

    if (token) {
      localStorage.setItem("token", token);
    }

    if (id) {
      localStorage.setItem("userId", id);
    }

    window.dispatchEvent(new Event('token-changed'));

    return response.data;
  } catch (error) {
    throw new Error(
      "Inloggning misslyckades: " + (error.response?.data?.message || error.message)
    );
  }
};

export const forgotPassword = async (email) => {
  try {
      return await apiClient.post('/Auth/forgot-password', {
        email: email
      });
  } catch (err) {
    throw err.response?.data?.message || 'Ett fel uppstod vid begäran om återställning.';
  }
};

export const resetPassword = async (token, newPassword) => {
  try {
    const response = await apiClient.post('/Auth/reset-password', {
      token,
      newPassword
    });
    return response.data;
  } catch (err) {
    throw err.response?.data?.message || 'Ett fel uppstod vid återställning av lösenord.';
  }
};

export const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("userId");

  window.dispatchEvent(new Event('token-changed'));
};

export const getToken = () => {
  return localStorage.getItem("token");
};

export const getUserId = () => {
  return localStorage.getItem("userId");
};

export const isLoggedIn = () => {
  return !!getToken();
};

// ======= COURSE =======

export const getAllCourses = async () => {
  const response = await apiClient.get("/Course/allCourses");
  return response.data.courses;
};

export const updateCourse = async (course) => {
  const userStore = useUserStore();
  if (userStore.isDemoUser()) {
    throw new Error('Demo-användare har inte rätt till att uppdatera banor.');
  }
  const response = await apiClient.put(`/Course/updateCourse`, course);
  return response.data.course;
};

export const addCourse = async (course) => {
  const userStore = useUserStore();
  if (userStore.isDemoUser()) {
    throw new Error('Demo-användare har inte rätt till att lägga till en bana.');
  }
  const response = await apiClient.post(`/Course/addCourse`, course);
  return response.data.course;
};

export const deleteCourse = async (courseId) => {
  const userStore = useUserStore();
  if (userStore.isDemoUser()) {
    throw new Error('Demo-användare har inte rätt till att ta bort en bana.');
  }
  const response = await apiClient.delete(`/Course/admin/deleteCourse/${courseId}`);
  return response.data.courseRemoved;
};

// ======= ROUND =======

export const updateRound = async (round) => {
  const userStore = useUserStore();
  if (userStore.isDemoUser()) {
    throw new Error('Demo-användare har inte rätt till att uppdatera en runda.');
  }
  const response = await apiClient.put(`/Round/updateRound`, round);
  return response.data.round;
};

export const addRound = async (round) => {
  const userStore = useUserStore();
  if (userStore.isDemoUser()) {
    throw new Error('Demo-användare har inte rätt till att lägga till en runda.');
  }
  try {
    const response = await apiClient.post(`/Round/addRound`, round);
    return response.data.round;
  } catch (error) {
    throw error;
  }
};
export const deleteRound = async (roundId) => {
  const userStore = useUserStore();
  if (userStore.isDemoUser()) {
    throw new Error('Demo-användare har inte rätt till att ta bort en runda.');
  }
  try {
    const response = await apiClient.delete(`/Round/deleteRound/${roundId}`);
    return response.data.roundRemoved;
  } catch (error) {
    throw error;
  }
};

// ======= USER =======

export const getUser = async (userId) => {
  try {
    const response = await apiClient.get(`/User/user`, {
      params: {
        userId: userId
      }
    });
    return response.data.user;
  } catch (error) {
    logout()
    throw new Error('Fel vid hämtning av användardata: ' + error.message);
  }
};

export const updateUser = async (userToUpdate) => {
  const userStore = useUserStore();
  if (userStore.isDemoUser()) {
    throw new Error('Demo-användare har inte rätt till att uppdatera user.');
  }
  try {
    const response = await apiClient.put(`/User/updateUser`, userToUpdate);
    return response.data.user;
  } catch (error) {
    logout()
    throw new Error('Fel vid hämtning av användardata: ' + error.message);
  }
};

export const updatePassword = async (oldPassword, newPassword, userId) => {
  const userStore = useUserStore();
  if (userStore.isDemoUser()) {
    throw new Error('Demo-användare har inte rätt till att uppdatera lösenord.');
  }
  try {
    const response = await apiClient.put(`/User/updatePassword`, {
      oldPassword,
      newPassword,
      userId
    });
    return response.data;
  } catch (error) {
    logout();
    throw new Error('Fel vid uppdatering av lösenord: ' + error.message);
  }
};
