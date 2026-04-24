const API_BASE_URL = "http://localhost:5031/api";

async function request(path, options = {}) {
  const response = await fetch(`${API_BASE_URL}${path}`, options);

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Request failed for ${path}`);
  }

  return response.json();
}

export const api = {
  getDashboard: () => request("/dashboard/summary"),
  getPets: () => request("/pets"),
  getAdoptions: () => request("/adoptions"),
  getLostPets: () => request("/community/lost"),
  getFoundPets: () => request("/community/found"),
  getUpcomingVaccines: () => request("/medical/upcoming-vaccines"),
  getNotifications: (userId) => request(`/community/notifications/${userId}`),
  login: (email, password) =>
    request("/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password })
    }),
  register: (payload) =>
    request("/auth/register", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload)
    })
};
