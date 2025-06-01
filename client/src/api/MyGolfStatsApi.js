import axios from "axios";

const API_BASE_URL = "https://localhost:7159/api";

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
  const response = await apiClient.put(`/Course/updateCourse`, course);
  return response.data.course;
};

export const addCourse = async (course) => {
  const response = await apiClient.post(`/Course/addCourse`, course);
  return response.data.course;
};

export const deleteCourse = async (courseId) => {
  const response = await apiClient.delete(`/Course/admin/deleteCourse/${courseId}`);
  return response.data.courseRemoved;
};

// ======= ROUND =======

export const updateRound = async (round) => {
  const response = await apiClient.put(`/Round/updateRound`, round);
  return response.data.round;
};

export const addRound = async (round) => {
  try {
    const response = await apiClient.post(`/Round/addRound`, round);
    return response.data.round;
  } catch (error) {
    throw error;
  }
};
export const deleteRound = async (roundId) => {
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
  try {
    const response = await apiClient.put(`/User/updateUser`, userToUpdate);
    return response.data.user;
  } catch (error) {
    logout()
    throw new Error('Fel vid hämtning av användardata: ' + error.message);
  }
};

export const updatePassword = async (oldPassword, newPassword, userId) => {
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
