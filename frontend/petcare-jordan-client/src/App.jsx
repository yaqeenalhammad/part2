import { useEffect, useState } from "react";
import { api } from "./api";

const tabs = [
  { id: "overview", label: "Overview" },
  { id: "adoption", label: "Adoption" },
  { id: "lostfound", label: "Lost & Found" },
  { id: "medical", label: "Medical" }
];

const roleOrder = ["User", "Vet", "Admin"];
const roleConfig = {
  User: {
    hint: "Adopt pets, publish updates, and track your own reports."
  },
  Vet: {
    hint: "Manage medical records and upcoming vaccine plans."
  },
  Admin: {
    hint: "Review pending posts and keep platform content approved."
  }
};

const demoCredentials = {
  User: { name: "Yaqeen Alhammad", email: "yaqeen.alhammad@petcare.com", password: "Pass123!" },
  Vet: { name: "Dr. Malak Alquraan", email: "malak.alquraan@petcare.com", password: "Pass123!" },
  Admin: { name: "Safaa Alquraan", email: "safaa.alquraan@petcare.com", password: "Pass123!" }
};

const emptyRegisterForms = {
  User: {
    fullName: "",
    email: "",
    password: "",
    phoneNumber: "",
    city: "",
    role: "User"
  },
  Vet: {
    fullName: "",
    email: "",
    password: "",
    phoneNumber: "",
    city: "",
    role: "Vet"
  }
};

const petTypeOptions = ["Cat", "Dog", "Bird", "Rabbit", "Other"];

function getNowLocalInputValue() {
  return new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 16);
}

function createInitialLostPostForm(currentUser) {
  return {
    petName: "",
    petType: "Cat",
    description: "",
    approximateAgeInMonths: "",
    lastSeenPlace: "",
    lastSeenDateUtc: getNowLocalInputValue(),
    rewardAmount: "",
    photoUrl: "",
    contactName: currentUser?.fullName ?? "",
    contactPhone: currentUser?.phoneNumber ?? ""
  };
}

function createInitialFoundPostForm(currentUser) {
  return {
    petType: "Cat",
    description: "",
    foundPlace: "",
    foundDateUtc: getNowLocalInputValue(),
    photoUrl: "",
    contactName: currentUser?.fullName ?? "",
    contactPhone: currentUser?.phoneNumber ?? ""
  };
}

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
  selectedRole,
  setSelectedRole,
  isRoleLocked,
  setIsRoleLocked,
  authModeByRole,
  setAuthModeByRole,
  loginForms,
  setLoginForms,
  registerForms,
  setRegisterForms,
  handleLogin,
  handleRegister,
  handleSignOut
}) {
  const selectedAuthMode = authModeByRole[selectedRole];
  const selectedLoginForm = loginForms[selectedRole];
  const selectedRegisterForm = registerForms[selectedRole];
  const canRegister = selectedRole !== "Admin";
  const visibleRolesInPanel = isRoleLocked ? [selectedRole] : roleOrder;

  return (
    <div className="login-panel">
      <div className="login-panel-header">
        <strong>{currentUser ? `Logged in as ${currentUser.fullName}` : "Choose your role to continue."}</strong>
        <span>{currentUser ? currentUser.role : "Pick the right access category for your account."}</span>
      </div>

      {!currentUser ? (
        <div className="role-list">
          {visibleRolesInPanel.map((role) => (
            <button
              key={role}
              type="button"
              className={selectedRole === role ? "role-entry active" : "role-entry"}
              onClick={() => {
                setSelectedRole(role);
                setIsRoleLocked(true);
                setAuthModeByRole((current) => ({ ...current, [role]: "login" }));
              }}
            >
              <div className="role-entry-main">
                <div>
                  <strong>{role}</strong>
                  <p>{roleConfig[role].hint}</p>
                </div>
              </div>
              <span className="role-arrow">{">"}</span>
            </button>
          ))}
        </div>
      ) : null}

      {!currentUser ? (
        <>
          {isRoleLocked ? (
            <>
              <button type="button" className="role-switch-action" onClick={() => setIsRoleLocked(false)}>
                Change role
              </button>

              {canRegister ? (
                <div className="auth-toggle">
                  <button
                    type="button"
                    className={selectedAuthMode === "login" ? "toggle active" : "toggle"}
                    onClick={() => setAuthModeByRole((current) => ({ ...current, [selectedRole]: "login" }))}
                  >
                    Login
                  </button>
                  <button
                    type="button"
                    className={selectedAuthMode === "register" ? "toggle active" : "toggle"}
                    onClick={() => setAuthModeByRole((current) => ({ ...current, [selectedRole]: "register" }))}
                  >
                    Register
                  </button>
                </div>
              ) : null}

              {selectedAuthMode === "login" || !canRegister ? (
                <form className="auth-form" onSubmit={handleLogin}>
                  <input
                    type="email"
                    placeholder="Email"
                    value={selectedLoginForm.email}
                    onChange={(event) =>
                      setLoginForms((current) => ({
                        ...current,
                        [selectedRole]: { ...current[selectedRole], email: event.target.value }
                      }))
                    }
                  />
                  <input
                    type="password"
                    placeholder="Password"
                    value={selectedLoginForm.password}
                    onChange={(event) =>
                      setLoginForms((current) => ({
                        ...current,
                        [selectedRole]: { ...current[selectedRole], password: event.target.value }
                      }))
                    }
                  />
                  <button type="submit">Sign in as {selectedRole}</button>
                </form>
              ) : (
                <form className="auth-form" onSubmit={handleRegister}>
                  <input
                    type="text"
                    placeholder="Full name"
                    value={selectedRegisterForm.fullName}
                    onChange={(event) =>
                      setRegisterForms((current) => ({
                        ...current,
                        [selectedRole]: { ...current[selectedRole], fullName: event.target.value }
                      }))
                    }
                  />
                  <input
                    type="email"
                    placeholder="Email (must end with @petcare.com)"
                    value={selectedRegisterForm.email}
                    onChange={(event) =>
                      setRegisterForms((current) => ({
                        ...current,
                        [selectedRole]: { ...current[selectedRole], email: event.target.value }
                      }))
                    }
                  />
                  <input
                    type="password"
                    placeholder="Password"
                    value={selectedRegisterForm.password}
                    onChange={(event) =>
                      setRegisterForms((current) => ({
                        ...current,
                        [selectedRole]: { ...current[selectedRole], password: event.target.value }
                      }))
                    }
                  />
                  <input
                    type="text"
                    placeholder="Phone number"
                    value={selectedRegisterForm.phoneNumber}
                    onChange={(event) =>
                      setRegisterForms((current) => ({
                        ...current,
                        [selectedRole]: { ...current[selectedRole], phoneNumber: event.target.value }
                      }))
                    }
                  />
                  <input
                    type="text"
                    placeholder="City"
                    value={selectedRegisterForm.city}
                    onChange={(event) =>
                      setRegisterForms((current) => ({
                        ...current,
                        [selectedRole]: { ...current[selectedRole], city: event.target.value }
                      }))
                    }
                  />
                  <button type="submit">Create {selectedRole} account</button>
                </form>
              )}

              {canRegister ? (
                <p className="auth-footnote">
                  {selectedAuthMode === "login" ? "No account yet? Choose Register." : "Already have an account? Choose Login."}
                </p>
              ) : null}
            </>
          ) : (
            <p className="auth-note">Select one role card to open its login flow.</p>
          )}
        </>
      ) : (
        <div className="signed-in-card">
          <p>{currentUser.email}</p>
          <p>{currentUser.city} | {currentUser.phoneNumber}</p>
          <button type="button" onClick={handleSignOut}>
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
  const [adoptions, setAdoptions] = useState([]);
  const [lostPets, setLostPets] = useState([]);
  const [foundPets, setFoundPets] = useState([]);
  const [vaccines, setVaccines] = useState([]);
  const [userMedicalPets, setUserMedicalPets] = useState([]);
  const [currentUser, setCurrentUser] = useState(() => {
    const stored = localStorage.getItem("petcareCurrentUser");
    return stored ? JSON.parse(stored) : null;
  });
  const [notifications, setNotifications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [privateLoading, setPrivateLoading] = useState(false);
  const [error, setError] = useState("");
  const [selectedRole, setSelectedRole] = useState("User");
  const [isRoleLocked, setIsRoleLocked] = useState(false);
  const [authModeByRole, setAuthModeByRole] = useState({
    User: "login",
    Vet: "login",
    Admin: "login"
  });
  const [loginForms, setLoginForms] = useState({
    User: { email: demoCredentials.User.email, password: demoCredentials.User.password },
    Vet: { email: demoCredentials.Vet.email, password: demoCredentials.Vet.password },
    Admin: { email: demoCredentials.Admin.email, password: demoCredentials.Admin.password }
  });
  const [registerForms, setRegisterForms] = useState({
    User: { ...emptyRegisterForms.User },
    Vet: { ...emptyRegisterForms.Vet }
  });
  const [lostPostForm, setLostPostForm] = useState(() => createInitialLostPostForm(currentUser));
  const [foundPostForm, setFoundPostForm] = useState(() => createInitialFoundPostForm(currentUser));
  const [lostFoundNotice, setLostFoundNotice] = useState("");
  const [lostPhotoUploading, setLostPhotoUploading] = useState(false);
  const [foundPhotoUploading, setFoundPhotoUploading] = useState(false);

  useEffect(() => {
    async function loadData() {
      try {
        const dashboardData = await api.getDashboard();
        setDashboard(dashboardData);
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
      localStorage.removeItem("petcareCurrentUser");
      setNotifications([]);
      return;
    }

    localStorage.setItem("petcareCurrentUser", JSON.stringify(currentUser));

    api.getNotifications(currentUser.id)
      .then(setNotifications)
      .catch(() => setNotifications([]));
  }, [currentUser]);

  useEffect(() => {
    if (!currentUser) {
      setAdoptions([]);
      setLostPets([]);
      setFoundPets([]);
      setVaccines([]);
      setUserMedicalPets([]);
      return;
    }

    let isCancelled = false;

    async function loadPrivateData() {
      try {
        setPrivateLoading(true);
        const medicalRequest =
          currentUser.role === "User"
            ? api.getMyMedicalPets(currentUser.token)
            : api.getUpcomingVaccines(currentUser.token);

        const [adoptionData, lostData, foundData, medicalData] = await Promise.all([
          api.getAdoptions(),
          api.getLostPets(),
          api.getFoundPets(),
          medicalRequest
        ]);

        if (isCancelled) {
          return;
        }

        setAdoptions(adoptionData);
        setLostPets(lostData);
        setFoundPets(foundData);
        if (currentUser.role === "User") {
          setUserMedicalPets(medicalData);
          setVaccines([]);
        } else {
          setVaccines(medicalData);
          setUserMedicalPets([]);
        }
      } catch {
        if (!isCancelled) {
          setError("Could not load account data. Please refresh after signing in.");
        }
      } finally {
        if (!isCancelled) {
          setPrivateLoading(false);
        }
      }
    }

    loadPrivateData();

    return () => {
      isCancelled = true;
    };
  }, [currentUser]);

  useEffect(() => {
    if (currentUser?.role) {
      setSelectedRole(currentUser.role);
      setIsRoleLocked(true);
    }
  }, [currentUser]);

  useEffect(() => {
    setLostPostForm(createInitialLostPostForm(currentUser));
    setFoundPostForm(createInitialFoundPostForm(currentUser));
    setLostFoundNotice("");
  }, [currentUser]);

  const visibleDemoRoles = isRoleLocked ? [selectedRole] : roleOrder;

  async function handleLogin(event) {
    event.preventDefault();
    try {
      const user = await api.login(loginForms[selectedRole].email, loginForms[selectedRole].password);
      if (user.role !== selectedRole) {
        setError(`This account is ${user.role}. Please use the ${user.role} login category.`);
        return;
      }
      setCurrentUser(user);
      setError("");
    } catch (loginError) {
      setError(loginError.message || "Login failed.");
    }
  }

  async function handleRegister(event) {
    event.preventDefault();
    if (selectedRole === "Admin") {
      setError("Admin accounts cannot be registered from this page.");
      return;
    }

    try {
      const payload = { ...registerForms[selectedRole], role: selectedRole };
      const user = await api.register(payload);
      setCurrentUser(user);
      setRegisterForms((current) => ({
        ...current,
        [selectedRole]: { ...emptyRegisterForms[selectedRole] }
      }));
      setError("");
    } catch (registerError) {
      setError(registerError.message || "Registration failed.");
    }
  }

  async function handleCreateLostReport(event) {
    event.preventDefault();

    if (!currentUser?.token) {
      setError("Please sign in to publish a lost pet report.");
      return;
    }

    if (currentUser.role !== "User") {
      setError("Only User accounts can publish lost or found posts.");
      return;
    }

    try {
      const payload = {
        ...lostPostForm,
        approximateAgeInMonths: Number(lostPostForm.approximateAgeInMonths),
        lastSeenDateUtc: new Date(lostPostForm.lastSeenDateUtc).toISOString(),
        rewardAmount: lostPostForm.rewardAmount ? Number(lostPostForm.rewardAmount) : null
      };
      await api.createLostPetReport(payload, currentUser.token);

      setLostPostForm(createInitialLostPostForm(currentUser));
      setLostFoundNotice("Lost pet post sent successfully. It will appear after admin approval.");
      setError("");
    } catch (createError) {
      setError(createError.message || "Could not submit the lost pet post.");
    }
  }

  async function handleCreateFoundReport(event) {
    event.preventDefault();

    if (!currentUser?.token) {
      setError("Please sign in to publish a found pet report.");
      return;
    }

    if (currentUser.role !== "User") {
      setError("Only User accounts can publish lost or found posts.");
      return;
    }

    try {
      const payload = {
        ...foundPostForm,
        foundDateUtc: new Date(foundPostForm.foundDateUtc).toISOString()
      };
      await api.createFoundPetReport(payload, currentUser.token);

      setFoundPostForm(createInitialFoundPostForm(currentUser));
      setLostFoundNotice("Found pet post sent successfully. It will appear after admin approval.");
      setError("");
    } catch (createError) {
      setError(createError.message || "Could not submit the found pet post.");
    }
  }

  async function handleUploadCommunityPhoto(file, setForm, setUploading) {
    if (!file) {
      return;
    }

    if (!currentUser?.token) {
      setError("Please sign in first to upload a photo.");
      return;
    }

    try {
      setUploading(true);
      const uploaded = await api.uploadCommunityImage(file, currentUser.token);
      setForm((current) => ({ ...current, photoUrl: uploaded.url }));
      setLostFoundNotice("Photo uploaded successfully. You can now submit your report.");
      setError("");
    } catch (uploadError) {
      setError(uploadError.message || "Could not upload this image.");
    } finally {
      setUploading(false);
    }
  }

  function handleSignOut() {
    setCurrentUser(null);
    setActiveTab("overview");
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
          <h3>Role Demo Accounts</h3>
          {visibleDemoRoles.map((role) => (
            <button
              key={role}
              type="button"
              className="credential-chip"
              onClick={() => {
                setSelectedRole(role);
                setIsRoleLocked(true);
                setAuthModeByRole((current) => ({ ...current, [role]: "login" }));
                setLoginForms((current) => ({
                  ...current,
                  [role]: { email: demoCredentials[role].email, password: demoCredentials[role].password }
                }));
              }}
            >
              {role}: {demoCredentials[role].name}
            </button>
          ))}
          {isRoleLocked ? (
            <button type="button" className="demo-switch" onClick={() => setIsRoleLocked(false)}>
              Show all role demos
            </button>
          ) : null}
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
            selectedRole={selectedRole}
            setSelectedRole={setSelectedRole}
            isRoleLocked={isRoleLocked}
            setIsRoleLocked={setIsRoleLocked}
            authModeByRole={authModeByRole}
            setAuthModeByRole={setAuthModeByRole}
            loginForms={loginForms}
            setLoginForms={setLoginForms}
            registerForms={registerForms}
            setRegisterForms={setRegisterForms}
            handleLogin={handleLogin}
            handleRegister={handleRegister}
            handleSignOut={handleSignOut}
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
                    <p className="empty-state">Sign in to see reminders for your account.</p>
                  )}
                </SectionCard>
              </div>
            ) : null}

            {activeTab === "adoption" && currentUser && !privateLoading ? (
              <SectionCard title="Adoption Marketplace" subtitle="Owners can publish pets for adoption and adopters can contact them directly.">
                <div className="pet-grid">
                  {adoptions.map((item) => (
                    <article key={item.id} className="pet-card">
                      <img src={item.photoUrl} alt={item.petName} />
                      <div className="pet-card-body">
                        <div className="pet-card-head">
                          <div>
                            <h4>{item.petName}</h4>
                            <span>{item.petType} | {item.breed}</span>
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

            {activeTab === "lostfound" && currentUser && !privateLoading ? (
              <div className="content-grid">
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

                <SectionCard title="Publish Lost / Found Report" subtitle="Your post is saved as Pending and appears publicly only after admin approval.">
                  {currentUser.role === "User" ? (
                    <div className="post-forms">
                      <form className="post-form" onSubmit={handleCreateLostReport}>
                        <h4>Report Lost Pet</h4>
                        <div className="form-grid-two">
                          <input
                            type="text"
                            placeholder="Pet name"
                            value={lostPostForm.petName}
                            onChange={(event) => setLostPostForm((current) => ({ ...current, petName: event.target.value }))}
                            required
                          />
                          <select
                            value={lostPostForm.petType}
                            onChange={(event) => setLostPostForm((current) => ({ ...current, petType: event.target.value }))}
                          >
                            {petTypeOptions.map((type) => (
                              <option key={type} value={type}>
                                {type}
                              </option>
                            ))}
                          </select>
                        </div>
                        <textarea
                          placeholder="Description"
                          value={lostPostForm.description}
                          onChange={(event) => setLostPostForm((current) => ({ ...current, description: event.target.value }))}
                          required
                        />
                        <div className="form-grid-two">
                          <input
                            type="number"
                            min="0"
                            placeholder="Approximate age (months)"
                            value={lostPostForm.approximateAgeInMonths}
                            onChange={(event) =>
                              setLostPostForm((current) => ({ ...current, approximateAgeInMonths: event.target.value }))
                            }
                            required
                          />
                          <input
                            type="number"
                            min="0"
                            step="0.5"
                            placeholder="Reward amount (optional)"
                            value={lostPostForm.rewardAmount}
                            onChange={(event) => setLostPostForm((current) => ({ ...current, rewardAmount: event.target.value }))}
                          />
                        </div>
                        <input
                          type="text"
                          placeholder="Last seen place"
                          value={lostPostForm.lastSeenPlace}
                          onChange={(event) => setLostPostForm((current) => ({ ...current, lastSeenPlace: event.target.value }))}
                          required
                        />
                        <input
                          type="datetime-local"
                          value={lostPostForm.lastSeenDateUtc}
                          onChange={(event) => setLostPostForm((current) => ({ ...current, lastSeenDateUtc: event.target.value }))}
                          required
                        />
                        <input
                          type="url"
                          placeholder="Photo URL"
                          value={lostPostForm.photoUrl}
                          onChange={(event) => setLostPostForm((current) => ({ ...current, photoUrl: event.target.value }))}
                          required
                        />
                        <input
                          type="file"
                          accept="image/png,image/jpeg,image/webp"
                          onChange={(event) =>
                            handleUploadCommunityPhoto(event.target.files?.[0], setLostPostForm, setLostPhotoUploading)
                          }
                        />
                        <p className="upload-hint">
                          {lostPhotoUploading
                            ? "Uploading image..."
                            : "You can paste a Photo URL or upload directly from your device."}
                        </p>
                        <div className="form-grid-two">
                          <input
                            type="text"
                            placeholder="Contact name"
                            value={lostPostForm.contactName}
                            onChange={(event) => setLostPostForm((current) => ({ ...current, contactName: event.target.value }))}
                            required
                          />
                          <input
                            type="text"
                            placeholder="Contact phone"
                            value={lostPostForm.contactPhone}
                            onChange={(event) => setLostPostForm((current) => ({ ...current, contactPhone: event.target.value }))}
                            required
                          />
                        </div>
                        <button type="submit">Submit Lost Report</button>
                      </form>

                      <form className="post-form" onSubmit={handleCreateFoundReport}>
                        <h4>Report Found Pet</h4>
                        <select
                          value={foundPostForm.petType}
                          onChange={(event) => setFoundPostForm((current) => ({ ...current, petType: event.target.value }))}
                        >
                          {petTypeOptions.map((type) => (
                            <option key={type} value={type}>
                              {type}
                            </option>
                          ))}
                        </select>
                        <textarea
                          placeholder="Description"
                          value={foundPostForm.description}
                          onChange={(event) => setFoundPostForm((current) => ({ ...current, description: event.target.value }))}
                          required
                        />
                        <input
                          type="text"
                          placeholder="Found place"
                          value={foundPostForm.foundPlace}
                          onChange={(event) => setFoundPostForm((current) => ({ ...current, foundPlace: event.target.value }))}
                          required
                        />
                        <input
                          type="datetime-local"
                          value={foundPostForm.foundDateUtc}
                          onChange={(event) => setFoundPostForm((current) => ({ ...current, foundDateUtc: event.target.value }))}
                          required
                        />
                        <input
                          type="url"
                          placeholder="Photo URL"
                          value={foundPostForm.photoUrl}
                          onChange={(event) => setFoundPostForm((current) => ({ ...current, photoUrl: event.target.value }))}
                          required
                        />
                        <input
                          type="file"
                          accept="image/png,image/jpeg,image/webp"
                          onChange={(event) =>
                            handleUploadCommunityPhoto(event.target.files?.[0], setFoundPostForm, setFoundPhotoUploading)
                          }
                        />
                        <p className="upload-hint">
                          {foundPhotoUploading
                            ? "Uploading image..."
                            : "You can paste a Photo URL or upload directly from your device."}
                        </p>
                        <div className="form-grid-two">
                          <input
                            type="text"
                            placeholder="Contact name"
                            value={foundPostForm.contactName}
                            onChange={(event) => setFoundPostForm((current) => ({ ...current, contactName: event.target.value }))}
                            required
                          />
                          <input
                            type="text"
                            placeholder="Contact phone"
                            value={foundPostForm.contactPhone}
                            onChange={(event) => setFoundPostForm((current) => ({ ...current, contactPhone: event.target.value }))}
                            required
                          />
                        </div>
                        <button type="submit">Submit Found Report</button>
                      </form>
                    </div>
                  ) : (
                    <p className="empty-state">Only User accounts can publish lost or found reports.</p>
                  )}

                  {lostFoundNotice ? <p className="form-success">{lostFoundNotice}</p> : null}
                </SectionCard>
              </div>
            ) : null}

            {activeTab === "medical" && currentUser && !privateLoading ? (
              currentUser.role === "User" ? (
                <SectionCard
                  title="My Pets Medical Status"
                  subtitle="Only your pets are shown here with health updates and vaccine plans."
                >
                  {userMedicalPets.length > 0 ? (
                    <div className="user-medical-grid">
                      {userMedicalPets.map((pet) => (
                        <article key={pet.petId} className="medical-pet-card">
                          <div className="medical-pet-head">
                            <div>
                              <h4>{pet.petName}</h4>
                              <span>{pet.petType} | {pet.breed}</span>
                            </div>
                            <span className={pet.isVaccinesUpToDate ? "pill success" : "pill warning"}>
                              {pet.isVaccinesUpToDate ? "Vaccines Up To Date" : `${pet.pendingVaccinesCount} Vaccine(s) Needed`}
                            </span>
                          </div>
                          <p className="pet-id-line">Pet ID: {pet.collarId}</p>

                          <p>{pet.healthSummary}</p>

                          <div className="medical-subsection">
                            <strong>Vaccine Plan</strong>
                            {pet.vaccinePlan.length > 0 ? (
                              <div className="list-stack">
                                {pet.vaccinePlan.map((vaccine) => (
                                  <article key={vaccine.id} className="list-card">
                                    <strong>{vaccine.vaccineName}</strong>
                                    <div className="meta-line">
                                      <span>Status: {vaccine.status}</span>
                                      <span>Due: {new Date(vaccine.dueDateUtc).toLocaleDateString()}</span>
                                    </div>
                                  </article>
                                ))}
                              </div>
                            ) : (
                              <p className="empty-state">No vaccine records available yet.</p>
                            )}
                          </div>

                          <div className="medical-subsection">
                            <strong>Recent Medical Visits</strong>
                            {pet.medicalHistory.length > 0 ? (
                              <div className="list-stack">
                                {pet.medicalHistory.slice(0, 3).map((record) => (
                                  <article key={record.id} className="list-card">
                                    <strong>{record.visitReason}</strong>
                                    <p>{record.diagnosis}</p>
                                    <div className="meta-line">
                                      <span>{record.vetName}</span>
                                      <span>{new Date(record.visitDateUtc).toLocaleDateString()}</span>
                                    </div>
                                  </article>
                                ))}
                              </div>
                            ) : (
                              <p className="empty-state">No medical visits recorded for this pet yet.</p>
                            )}
                          </div>
                        </article>
                      ))}
                    </div>
                  ) : (
                    <p className="empty-state">No pets are linked to your account yet.</p>
                  )}
                </SectionCard>
              ) : (
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
                            <span>Pet ID: {item.petCollarId}</span>
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
              )
            ) : null}

            {activeTab !== "overview" && !currentUser ? (
              <SectionCard
                title="Login Required"
                subtitle="Please sign in first,This section is visible only after signing in"
              />
            ) : null}

            {activeTab !== "overview" && currentUser && privateLoading ? (
              <div className="section-card">Loading your account data...</div>
            ) : null}
          </>
        ) : null}
      </main>
    </div>
  );
}

export default App;
