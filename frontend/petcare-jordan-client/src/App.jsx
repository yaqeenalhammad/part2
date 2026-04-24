import { useEffect, useMemo, useState } from "react";
import { api } from "./api";

const tabs = [
  { id: "overview", label: "Overview" },
  { id: "adoption", label: "Adoption" },
  { id: "lostfound", label: "Lost & Found" },
  { id: "medical", label: "Medical" },
  { id: "directory", label: "Pet Directory" }
];

const demoCredentials = [
  { role: "Admin", email: "alaa@petcare.jo", password: "Pass123!" },
  { role: "Vet", email: "noor.vet@petcare.jo", password: "Pass123!" },
  { role: "User", email: "lina@petcare.jo", password: "Pass123!" }
];

const emptyRegisterForm = {
  fullName: "",
  email: "",
  password: "",
  phoneNumber: "",
  city: "",
  role: "User"
};

function StatCard({ label, value, accent }) {
  return (
    <div className="stat-card">
      <span className="stat-label">{label}</span>
      <strong style={{ color: accent }}>{value}</strong>
    </div>
  );
}

function SectionCard({ title, subtitle, children }) {
  return (
    <section className="section-card">
      <div className="section-heading">
        <div>
          <h3>{title}</h3>
          {subtitle ? <p>{subtitle}</p> : null}
        </div>
      </div>
      {children}
    </section>
  );
}

function AuthPanel({
  currentUser,
  authMode,
  setAuthMode,
  loginForm,
  setLoginForm,
  registerForm,
  setRegisterForm,
  handleLogin,
  handleRegister
}) {
  return (
    <div className="login-panel">
      <div className="login-panel-header">
        <strong>{currentUser ? `Logged in as ${currentUser.fullName}` : "Account Access"}</strong>
        <span>{currentUser ? currentUser.role : "Create an account or sign in"}</span>
      </div>

      {!currentUser ? (
        <>
          <div className="auth-toggle">
            <button type="button" className={authMode === "login" ? "toggle active" : "toggle"} onClick={() => setAuthMode("login")}>
              Login
            </button>
            <button type="button" className={authMode === "register" ? "toggle active" : "toggle"} onClick={() => setAuthMode("register")}>
              Register
            </button>
          </div>

          {authMode === "login" ? (
            <form className="auth-form" onSubmit={handleLogin}>
              <input
                type="email"
                placeholder="Email"
                value={loginForm.email}
                onChange={(event) => setLoginForm((current) => ({ ...current, email: event.target.value }))}
              />
              <input
                type="password"
                placeholder="Password"
                value={loginForm.password}
                onChange={(event) => setLoginForm((current) => ({ ...current, password: event.target.value }))}
              />
              <button type="submit">Sign in</button>
            </form>
          ) : (
            <form className="auth-form" onSubmit={handleRegister}>
              <input
                type="text"
                placeholder="Full name"
                value={registerForm.fullName}
                onChange={(event) => setRegisterForm((current) => ({ ...current, fullName: event.target.value }))}
              />
              <input
                type="email"
                placeholder="Email"
                value={registerForm.email}
                onChange={(event) => setRegisterForm((current) => ({ ...current, email: event.target.value }))}
              />
              <input
                type="password"
                placeholder="Password"
                value={registerForm.password}
                onChange={(event) => setRegisterForm((current) => ({ ...current, password: event.target.value }))}
              />
              <input
                type="text"
                placeholder="Phone number"
                value={registerForm.phoneNumber}
                onChange={(event) => setRegisterForm((current) => ({ ...current, phoneNumber: event.target.value }))}
              />
              <input
                type="text"
                placeholder="City"
                value={registerForm.city}
                onChange={(event) => setRegisterForm((current) => ({ ...current, city: event.target.value }))}
              />
              <select
                value={registerForm.role}
                onChange={(event) => setRegisterForm((current) => ({ ...current, role: event.target.value }))}
              >
                <option value="User">User</option>
                <option value="Vet">Vet</option>
              </select>
              <button type="submit">Create account</button>
            </form>
          )}
        </>
      ) : (
        <div className="signed-in-card">
          <p>{currentUser.email}</p>
          <p>{currentUser.city} • {currentUser.phoneNumber}</p>
          <button type="button" onClick={() => {
            localStorage.removeItem("petcareCurrentUser");
            setAuthMode("login");
            window.location.reload();
          }}>
            Sign out
          </button>
        </div>
      )}
    </div>
  );
}

function App() {
  const [activeTab, setActiveTab] = useState("overview");
  const [dashboard, setDashboard] = useState(null);
  const [pets, setPets] = useState([]);
  const [adoptions, setAdoptions] = useState([]);
  const [lostPets, setLostPets] = useState([]);
  const [foundPets, setFoundPets] = useState([]);
  const [vaccines, setVaccines] = useState([]);
  const [currentUser, setCurrentUser] = useState(() => {
    const stored = localStorage.getItem("petcareCurrentUser");
    return stored ? JSON.parse(stored) : null;
  });
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  const [authMode, setAuthMode] = useState("login");
  const [loginForm, setLoginForm] = useState({
    email: demoCredentials[0].email,
    password: demoCredentials[0].password
  });
  const [registerForm, setRegisterForm] = useState(emptyRegisterForm);

  useEffect(() => {
    async function loadData() {
      try {
        const [dashboardData, petsData, adoptionData, lostData, foundData, vaccineData] = await Promise.all([
          api.getDashboard(),
          api.getPets(),
          api.getAdoptions(),
          api.getLostPets(),
          api.getFoundPets(),
          api.getUpcomingVaccines()
        ]);

        setDashboard(dashboardData);
        setPets(petsData);
        setAdoptions(adoptionData);
        setLostPets(lostData);
        setFoundPets(foundData);
        setVaccines(vaccineData);
      } catch {
        setError("Could not load the API. Start the backend first, then refresh the page.");
      } finally {
        setLoading(false);
      }
    }

    loadData();
  }, []);

  useEffect(() => {
    if (!currentUser) {
      setNotifications([]);
      return;
    }

    localStorage.setItem("petcareCurrentUser", JSON.stringify(currentUser));

    api.getNotifications(currentUser.id)
      .then(setNotifications)
      .catch(() => setNotifications([]));
  }, [currentUser]);

  const filteredPets = useMemo(() => {
    return pets.filter((pet) => {
      const target = `${pet.name} ${pet.breed} ${pet.city} ${pet.collarId}`.toLowerCase();
      return target.includes(searchTerm.toLowerCase());
    });
  }, [pets, searchTerm]);

  async function handleLogin(event) {
    event.preventDefault();
    try {
      const user = await api.login(loginForm.email, loginForm.password);
      setCurrentUser(user);
      setError("");
    } catch (loginError) {
      setError(loginError.message || "Login failed.");
    }
  }

  async function handleRegister(event) {
    event.preventDefault();
    try {
      const user = await api.register(registerForm);
      setCurrentUser(user);
      setRegisterForm(emptyRegisterForm);
      setError("");
    } catch (registerError) {
      setError(registerError.message || "Registration failed.");
    }
  }

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div>
          <p className="eyebrow">Graduation Project</p>
          <h1>PetCare Jordan</h1>
          <p className="sidebar-copy">
            A pet adoption, recovery, and veterinary care platform built with ASP.NET Core and React.
          </p>
        </div>

        <nav className="tab-list">
          {tabs.map((tab) => (
            <button
              key={tab.id}
              type="button"
              className={activeTab === tab.id ? "tab active" : "tab"}
              onClick={() => setActiveTab(tab.id)}
            >
              {tab.label}
            </button>
          ))}
        </nav>

        <div className="demo-card">
          <h3>Demo Accounts</h3>
          {demoCredentials.map((item) => (
            <button
              key={item.role}
              type="button"
              className="credential-chip"
              onClick={() => {
                setAuthMode("login");
                setLoginForm({ email: item.email, password: item.password });
              }}
            >
              {item.role}: {item.email}
            </button>
          ))}
        </div>
      </aside>

      <main className="main-content">
        <header className="hero">
          <div>
            <p className="eyebrow">Jordan-wide care network</p>
            <h2>Manage adoptions, lost pets, pet health history, and vaccine reminders in one place.</h2>
          </div>

          <AuthPanel
            currentUser={currentUser}
            authMode={authMode}
            setAuthMode={setAuthMode}
            loginForm={loginForm}
            setLoginForm={setLoginForm}
            registerForm={registerForm}
            setRegisterForm={setRegisterForm}
            handleLogin={handleLogin}
            handleRegister={handleRegister}
          />
        </header>

        {error ? <div className="alert">{error}</div> : null}
        {loading ? <div className="section-card">Loading project data...</div> : null}

        {!loading && dashboard ? (
          <>
            {activeTab === "overview" ? (
              <div className="content-grid">
                <SectionCard title="Analytics Dashboard" subtitle="A quick project snapshot for admins and supervisors.">
                  <div className="stats-grid">
                    <StatCard label="Registered users" value={dashboard.totalUsers} accent="#0f766e" />
                    <StatCard label="Veterinarians" value={dashboard.totalVets} accent="#a16207" />
                    <StatCard label="Pets in system" value={dashboard.totalPets} accent="#0f172a" />
                    <StatCard label="Pets for adoption" value={dashboard.petsForAdoption} accent="#c2410c" />
                    <StatCard label="Active lost reports" value={dashboard.lostReports} accent="#be123c" />
                    <StatCard label="Upcoming vaccines" value={dashboard.upcomingVaccines} accent="#1d4ed8" />
                  </div>
                </SectionCard>

                <SectionCard title="Pets By Type" subtitle="Distribution of demo pets across the platform.">
                  <div className="bar-list">
                    {Object.entries(dashboard.petsByType).map(([label, value]) => (
                      <div key={label} className="bar-row">
                        <span>{label}</span>
                        <div className="bar-track">
                          <div className="bar-fill" style={{ width: `${(value / dashboard.totalPets) * 100}%` }} />
                        </div>
                        <strong>{value}</strong>
                      </div>
                    ))}
                  </div>
                </SectionCard>

                <SectionCard title="Pets By City" subtitle="Jordanian city coverage for the seeded pet data.">
                  <div className="city-grid">
                    {Object.entries(dashboard.petsByCity).map(([city, value]) => (
                      <article key={city} className="city-card">
                        <strong>{city}</strong>
                        <span>{value} pets</span>
                      </article>
                    ))}
                  </div>
                </SectionCard>

                <SectionCard title="Owner Notifications" subtitle="Vaccine reminders that owners receive before due dates.">
                  {notifications.length > 0 ? (
                    <div className="list-stack">
                      {notifications.map((item) => (
                        <article key={item.id} className="list-card">
                          <strong>{item.title}</strong>
                          <p>{item.message}</p>
                        </article>
                      ))}
                    </div>
                  ) : (
                    <p className="empty-state">Sign in or register to see reminders for your account.</p>
                  )}
                </SectionCard>
              </div>
            ) : null}

            {activeTab === "adoption" ? (
              <SectionCard title="Adoption Marketplace" subtitle="Owners can publish pets for adoption and adopters can contact them directly.">
                <div className="pet-grid">
                  {adoptions.map((item) => (
                    <article key={item.id} className="pet-card">
                      <img src={item.photoUrl} alt={item.petName} />
                      <div className="pet-card-body">
                        <div className="pet-card-head">
                          <div>
                            <h4>{item.petName}</h4>
                            <span>{item.petType} • {item.breed}</span>
                          </div>
                          <span className={item.status === "Available" ? "pill success" : "pill warning"}>{item.status}</span>
                        </div>
                        <p>{item.story}</p>
                        <div className="meta-line">
                          <span>{item.city}</span>
                          <span>{item.contactMethod}: {item.contactDetails}</span>
                        </div>
                      </div>
                    </article>
                  ))}
                </div>
              </SectionCard>
            ) : null}

            {activeTab === "lostfound" ? (
              <div className="content-grid two-column">
                <SectionCard title="Lost Pets" subtitle="Community notices for missing pets.">
                  <div className="list-stack">
                    {lostPets.map((item) => (
                      <article key={item.id} className="list-card">
                        <strong>{item.petName}</strong>
                        <p>{item.description}</p>
                        <div className="meta-line">
                          <span>{item.lastSeenPlace}</span>
                          <span>{new Date(item.lastSeenDateUtc).toLocaleDateString()}</span>
                        </div>
                        <div className="meta-line">
                          <span>Reward: {item.rewardAmount ? `${item.rewardAmount} JOD` : "No reward listed"}</span>
                          <span>{item.contactPhone}</span>
                        </div>
                      </article>
                    ))}
                  </div>
                </SectionCard>

                <SectionCard title="Found Pets" subtitle="Reports for pets that have been found and are waiting to be matched.">
                  <div className="list-stack">
                    {foundPets.map((item) => (
                      <article key={item.id} className="list-card">
                        <strong>{item.petType}</strong>
                        <p>{item.description}</p>
                        <div className="meta-line">
                          <span>{item.foundPlace}</span>
                          <span>{new Date(item.foundDateUtc).toLocaleDateString()}</span>
                        </div>
                        <div className="meta-line">
                          <span>Contact: {item.contactName}</span>
                          <span>{item.contactPhone}</span>
                        </div>
                      </article>
                    ))}
                  </div>
                </SectionCard>
              </div>
            ) : null}

            {activeTab === "medical" ? (
              <div className="content-grid two-column">
                <SectionCard title="Upcoming Vaccines" subtitle="This list powers the vaccine reminder workflow for owners.">
                  <div className="list-stack">
                    {vaccines.map((item) => (
                      <article key={item.id} className="list-card">
                        <strong>{item.petName}</strong>
                        <p>{item.vaccineName}</p>
                        <div className="meta-line">
                          <span>Owner: {item.ownerName}</span>
                          <span>Due: {new Date(item.dueDateUtc).toLocaleDateString()}</span>
                        </div>
                        <div className="meta-line">
                          <span>{item.ownerPhone}</span>
                        </div>
                      </article>
                    ))}
                  </div>
                </SectionCard>

                <SectionCard title="Vet Role Story" subtitle="How the medical workflow works in this project.">
                  <div className="feature-list">
                    <div>
                      <strong>Create medical history</strong>
                      <p>Veterinarians can create visit notes, diagnoses, and treatment records for each pet.</p>
                    </div>
                    <div>
                      <strong>Update records</strong>
                      <p>Vets can edit existing medical entries as treatment plans change over time.</p>
                    </div>
                    <div>
                      <strong>Track vaccines</strong>
                      <p>The backend identifies vaccines due in the next 30 days and surfaces reminders for owners.</p>
                    </div>
                    <div>
                      <strong>Search by collar ID</strong>
                      <p>Each pet can be found quickly through its unique collar ID to access ownership and health information.</p>
                    </div>
                  </div>
                </SectionCard>
              </div>
            ) : null}

            {activeTab === "directory" ? (
              <SectionCard title="Pet Directory" subtitle="Search the demo pets by name, breed, city, or collar ID.">
                <div className="search-row">
                  <input
                    type="search"
                    placeholder="Try PCJ-1001, rabbit, Amman..."
                    value={searchTerm}
                    onChange={(event) => setSearchTerm(event.target.value)}
                  />
                </div>
                <div className="table-shell">
                  <table>
                    <thead>
                      <tr>
                        <th>Name</th>
                        <th>Type</th>
                        <th>Breed</th>
                        <th>City</th>
                        <th>Collar ID</th>
                        <th>Owner</th>
                        <th>Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      {filteredPets.map((pet) => (
                        <tr key={pet.id}>
                          <td>{pet.name}</td>
                          <td>{pet.type}</td>
                          <td>{pet.breed}</td>
                          <td>{pet.city}</td>
                          <td>{pet.collarId}</td>
                          <td>{pet.ownerName}</td>
                          <td>{pet.adoptionStatus ?? "Not listed"}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </SectionCard>
            ) : null}
          </>
        ) : null}
      </main>
    </div>
  );
}

export default App;
