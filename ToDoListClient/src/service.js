import axios from 'axios';

const apiUrl = process.env.REACT_APP_API_URL;

console.log('Using API URL:', apiUrl);
axios.defaults.baseURL = apiUrl;
console.log('API URL:', apiUrl);
console.log('Using API URL:', apiUrl);
axios.defaults.baseURL = apiUrl;
console.log('API URL:', apiUrl);

axios.interceptors.response.use(
  response => response,
  error => {
    console.error('API Error:', error.response ? error.response.data : error.message);
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get('/items');
    return result.data;
  },

  addTask: async (name) => {
    console.log('addTask', name);
    await axios.post('/items', { name: name, isComplete: false });

  },
  setCompleted: async (id, isComplete) => {
    try {
      console.log('setCompleted', { id, isComplete });
      const result = await axios.put(`/items/${id}`, { isComplete });
      return result.data;
    } catch (error) {
      console.error('Failed to update task:', error);
      throw error;
    }
  },

  deleteTask: async (id) => {
    await axios.delete(`/items/${id}`);
  }
};






