import { useEffect, useState } from "react";
import { api } from "./api";

const tabs = [
  { id: "overview", label: "Overview" },
  { id: "adoption", label: "Adoption" },
  { id: "lostfound", label: "Lost & Found" },
  { id: "chat", label: "Chat" },
  { id: "medical", label: "Medical" }
];
const adminHiddenTabs = new Set(["chat", "medical"]);

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
const jordanTimeFormatter = new Intl.DateTimeFormat("en-JO", {
  dateStyle: "medium",
  timeStyle: "short",
  timeZone: "Asia/Amman"
});

function formatJordanDateTime(value) {
  const textValue = String(value);
  const utcValue = /(?:Z|[+-]\d{2}:\d{2})$/.test(textValue) ? textValue : `${textValue}Z`;
  return jordanTimeFormatter.format(new Date(utcValue));
}

function createInitialAdoptionPostForm(currentUser) {
  return {
    petName: "",
    petType: "Cat",
    weightKg: "",
    city: currentUser?.city ?? "",
    photoUrl: "",
    description: "",
    contactPhone: currentUser?.phoneNumber ?? ""
  };
}

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

const jordanCityPositions = {
  Irbid: { x: 22, y: 25 },
  Ajloun: { x: 17, y: 32 },
  Jerash: { x: 29, y: 35 },
  Mafraq: { x: 60, y: 28 },
  Salt: { x: 24, y: 47 },
  Amman: { x: 40, y: 44 },
  Zarqa: { x: 54, y: 43 },
  Madaba: { x: 39, y: 56 },
  Karak: { x: 47, y: 67 },
  Tafilah: { x: 40, y: 77 },
  Maan: { x: 50, y: 76 },
  Aqaba: { x: 29, y: 87 }
};

function JordanPetsMap({ petsByCity }) {
  const entries = Object.entries(petsByCity);
  const maxPets = Math.max(...entries.map(([, value]) => value), 1);
  const mappedCities = entries.filter(([city]) => jordanCityPositions[city]);

  return (
    <div className="jordan-map-layout">
      <div className="jordan-map-panel" aria-label="Jordan pets by city map">
        <img className="jordan-map-template" src="/jordan-map-template.jfif" alt="Jordan map outline" />

        {mappedCities.map(([city, value]) => {
          const position = jordanCityPositions[city];
          const markerSize = 18 + (value / maxPets) * 6;
          return (
            <div
              key={city}
              className="map-city-pin"
              style={{
                left: `${position.x}%`,
                top: `${position.y}%`,
                "--pin-size": `${markerSize}px`
              }}
              title={`${city}: ${value} pets`}
            >
              <span className="map-pin-head">
                <strong>{value}</strong>
              </span>
              <span>{city}</span>
            </div>
          );
        })}
      </div>
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
  const [adminAdoptions, setAdminAdoptions] = useState([]);
  const [lostPets, setLostPets] = useState([]);
  const [foundPets, setFoundPets] = useState([]);
  const [pendingLostPets, setPendingLostPets] = useState([]);
  const [pendingFoundPets, setPendingFoundPets] = useState([]);
  const [myLostPets, setMyLostPets] = useState([]);
  const [myFoundPets, setMyFoundPets] = useState([]);
  const [vaccines, setVaccines] = useState([]);
  const [userMedicalPets, setUserMedicalPets] = useState([]);
  const [chatVets, setChatVets] = useState([]);
  const [chatConversations, setChatConversations] = useState([]);
  const [selectedConversationId, setSelectedConversationId] = useState(null);
  const [chatMessages, setChatMessages] = useState([]);
  const [chatMessageDraft, setChatMessageDraft] = useState("");
  const [chatNotice, setChatNotice] = useState("");
  const [chatLoading, setChatLoading] = useState(false);
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
  const [adoptionPostForm, setAdoptionPostForm] = useState(() => createInitialAdoptionPostForm(currentUser));
  const [adoptionNotice, setAdoptionNotice] = useState("");
  const [brokenAdoptionImages, setBrokenAdoptionImages] = useState({});
  const [lostFoundNotice, setLostFoundNotice] = useState("");
  const [adoptionPhotoUploading, setAdoptionPhotoUploading] = useState(false);
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
      setAdminAdoptions([]);
      setLostPets([]);
      setFoundPets([]);
      setPendingLostPets([]);
      setPendingFoundPets([]);
      setMyLostPets([]);
      setMyFoundPets([]);
      setVaccines([]);
      setUserMedicalPets([]);
      setChatVets([]);
      setChatConversations([]);
      setSelectedConversationId(null);
      setChatMessages([]);
      setChatMessageDraft("");
      setChatNotice("");
      return;
    }

    let isCancelled = false;

    async function loadPrivateData() {
      try {
        setPrivateLoading(true);
        const isAdmin = currentUser.role === "Admin";
        const medicalRequest =
          isAdmin
            ? Promise.resolve([])
            : currentUser.role === "User"
            ? api.getMyMedicalPets(currentUser.token)
            : api.getUpcomingVaccines(currentUser.token);
        const chatConversationsRequest = isAdmin
          ? Promise.resolve([])
          : api.getMyChatConversations(currentUser.token);
        const chatVetsRequest = currentUser.role === "User" && !isAdmin
          ? api.getChatVets(currentUser.token)
          : Promise.resolve([]);
        const adoptionRequest = isAdmin
          ? api.getAdminAdoptions(currentUser.token)
          : api.getAdoptions();
        const pendingCommunityRequest = isAdmin
          ? api.getPendingCommunityReports(currentUser.token)
          : Promise.resolve({ lostReports: [], foundReports: [] });
        const myCommunityRequest = !isAdmin && (currentUser.role === "User" || currentUser.role === "Vet")
          ? api.getMyCommunityReports(currentUser.token)
          : Promise.resolve({ lostReports: [], foundReports: [] });

        const [adoptionData, lostData, foundData, medicalData, conversationData, vetsData, pendingCommunityData, myCommunityData] = await Promise.all([
          adoptionRequest,
          api.getLostPets(),
          api.getFoundPets(),
          medicalRequest,
          chatConversationsRequest,
          chatVetsRequest,
          pendingCommunityRequest,
          myCommunityRequest
        ]);

        if (isCancelled) {
          return;
        }

        setAdoptions(adoptionData);
        setAdminAdoptions(isAdmin ? adoptionData : []);
        setLostPets(lostData);
        setFoundPets(foundData);
        setPendingLostPets(pendingCommunityData.lostReports ?? []);
        setPendingFoundPets(pendingCommunityData.foundReports ?? []);
        setMyLostPets(myCommunityData.lostReports ?? []);
        setMyFoundPets(myCommunityData.foundReports ?? []);
        setChatConversations(conversationData);
        setChatVets(vetsData);
        setSelectedConversationId((current) => {
          if (isAdmin) {
            return null;
          }

          if (!current && conversationData.length > 0) {
            return conversationData[0].id;
          }

          if (current && !conversationData.some((item) => item.id === current)) {
            return conversationData[0]?.id ?? null;
          }

          return current;
        });
        if (isAdmin) {
          setVaccines([]);
          setUserMedicalPets([]);
        } else if (currentUser.role === "User") {
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
    setAdoptionPostForm(createInitialAdoptionPostForm(currentUser));
    setLostPostForm(createInitialLostPostForm(currentUser));
    setFoundPostForm(createInitialFoundPostForm(currentUser));
    setAdoptionNotice("");
    setLostFoundNotice("");
  }, [currentUser]);

  useEffect(() => {
    const isChatRole = currentUser?.role === "User" || currentUser?.role === "Vet";
    if (!currentUser?.token || !isChatRole) {
      return;
    }

    let isCancelled = false;

    async function syncConversations() {
      try {
        const conversations = await api.getMyChatConversations(currentUser.token);
        if (isCancelled) {
          return;
        }

        setChatConversations(conversations);
        setSelectedConversationId((current) => {
          if (current && conversations.some((item) => item.id === current)) {
            return current;
          }
          return conversations[0]?.id ?? null;
        });

        if (currentUser.role === "User") {
          const vets = await api.getChatVets(currentUser.token);
          if (!isCancelled) {
            setChatVets(vets);
          }
        }
      } catch {
        if (!isCancelled) {
          setError("Could not refresh chat list.");
        }
      }
    }

    syncConversations();
    const timerId = window.setInterval(syncConversations, 5000);

    return () => {
      isCancelled = true;
      window.clearInterval(timerId);
    };
  }, [currentUser]);

  useEffect(() => {
    if (!currentUser?.token || !selectedConversationId || activeTab !== "chat") {
      setChatMessages([]);
      return;
    }

    let isCancelled = false;

    async function syncMessages() {
      try {
        setChatLoading(true);
        const messages = await api.getChatMessages(selectedConversationId, currentUser.token);
        if (!isCancelled) {
          setChatMessages(messages);
        }
      } catch {
        if (!isCancelled) {
          setError("Could not load chat messages.");
        }
      } finally {
        if (!isCancelled) {
          setChatLoading(false);
        }
      }
    }

    syncMessages();
    const timerId = window.setInterval(syncMessages, 3000);

    return () => {
      isCancelled = true;
      window.clearInterval(timerId);
    };
  }, [activeTab, currentUser, selectedConversationId]);

  useEffect(() => {
    if (activeTab !== "chat" || !selectedConversationId) {
      return;
    }

    setChatConversations((current) =>
      current.map((conversation) =>
        conversation.id === selectedConversationId
          ? { ...conversation, unreadIncomingCount: 0 }
          : conversation
      )
    );
  }, [activeTab, selectedConversationId]);

  const visibleDemoRoles = isRoleLocked ? [selectedRole] : roleOrder;
  const selectedConversation = chatConversations.find((item) => item.id === selectedConversationId) ?? null;
  const vetsWithoutConversation = chatVets.filter(
    (vet) => !chatConversations.some((conversation) => conversation.vetId === vet.id)
  );
  const isChatRole = currentUser?.role === "User" || currentUser?.role === "Vet";
  const canPublishCommunityPost = currentUser?.role === "User" || currentUser?.role === "Vet";
  const pendingAdoptions = adminAdoptions.filter((item) => item.status === "Pending");
  const publishedAdoptions = adminAdoptions.filter((item) => item.status === "Available");
  const rejectedAdoptions = adminAdoptions.filter((item) => item.status === "Rejected");
  const communityLostPets = canPublishCommunityPost
    ? lostPets.filter((item) => item.reporterId !== currentUser.id)
    : lostPets;
  const communityFoundPets = canPublishCommunityPost
    ? foundPets.filter((item) => item.reporterId !== currentUser.id)
    : foundPets;
  const chatUnreadCount = chatConversations.reduce(
    (total, conversation) => total + (conversation.unreadIncomingCount ?? 0),
    0
  );
  const visibleTabs = currentUser?.role === "Admin"
    ? tabs.filter((tab) => !adminHiddenTabs.has(tab.id))
    : tabs;

  useEffect(() => {
    if (!visibleTabs.some((tab) => tab.id === activeTab)) {
      setActiveTab("overview");
    }
  }, [activeTab, visibleTabs]);

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

  async function handleCreateAdoptionPost(event) {
    event.preventDefault();

    if (!currentUser?.token) {
      setError("Please sign in to publish an adoption post.");
      return;
    }

    if (!canPublishCommunityPost) {
      setError("Only User and Vet accounts can publish adoption posts.");
      return;
    }

    try {
      const payload = {
        ...adoptionPostForm,
        weightKg: Number(adoptionPostForm.weightKg)
      };
      await api.createAdoptionPost(payload, currentUser.token);

      setAdoptionPostForm(createInitialAdoptionPostForm(currentUser));
      setAdoptionNotice("Adoption post sent successfully. It will appear after admin approval.");
      setError("");
    } catch (createError) {
      setError(createError.message || "Could not submit the adoption post.");
    }
  }

  async function handleCreateLostReport(event) {
    event.preventDefault();

    if (!currentUser?.token) {
      setError("Please sign in to publish a lost pet report.");
      return;
    }

    if (!canPublishCommunityPost) {
      setError("Only User and Vet accounts can publish lost or found posts.");
      return;
    }

    try {
      const payload = {
        ...lostPostForm,
        approximateAgeInMonths: Number(lostPostForm.approximateAgeInMonths),
        lastSeenDateUtc: new Date(lostPostForm.lastSeenDateUtc).toISOString(),
        rewardAmount: lostPostForm.rewardAmount ? Number(lostPostForm.rewardAmount) : null
      };
      const createdReport = await api.createLostPetReport(payload, currentUser.token);

      setMyLostPets((current) => [createdReport, ...current]);
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

    if (!canPublishCommunityPost) {
      setError("Only User and Vet accounts can publish lost or found posts.");
      return;
    }

    try {
      const payload = {
        ...foundPostForm,
        foundDateUtc: new Date(foundPostForm.foundDateUtc).toISOString()
      };
      const createdReport = await api.createFoundPetReport(payload, currentUser.token);

      setMyFoundPets((current) => [createdReport, ...current]);
      setFoundPostForm(createInitialFoundPostForm(currentUser));
      setLostFoundNotice("Found pet post sent successfully. It will appear after admin approval.");
      setError("");
    } catch (createError) {
      setError(createError.message || "Could not submit the found pet post.");
    }
  }

  async function handleUploadCommunityPhoto(file, setForm, setUploading, setNotice = setLostFoundNotice) {
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
      setNotice("Photo uploaded successfully. You can now submit your post.");
      setError("");
    } catch (uploadError) {
      setError(uploadError.message || "Could not upload this image.");
    } finally {
      setUploading(false);
    }
  }

  async function handleReviewCommunityReport(kind, id, decision) {
    if (!currentUser?.token || currentUser.role !== "Admin") {
      setError("Only Admin accounts can review lost and found posts.");
      return;
    }

    try {
      if (kind === "lost") {
        if (decision === "approve") {
          await api.approveLostPetReport(id, currentUser.token);
        } else {
          await api.rejectLostPetReport(id, currentUser.token);
        }
        setPendingLostPets((current) => current.filter((item) => item.id !== id));
      } else {
        if (decision === "approve") {
          await api.approveFoundPetReport(id, currentUser.token);
        } else {
          await api.rejectFoundPetReport(id, currentUser.token);
        }
        setPendingFoundPets((current) => current.filter((item) => item.id !== id));
      }

      const publicReports = await Promise.all([api.getLostPets(), api.getFoundPets()]);
      setLostPets(publicReports[0]);
      setFoundPets(publicReports[1]);
      setLostFoundNotice(decision === "approve" ? "Post approved and published." : "Post rejected.");
      setError("");
    } catch (reviewError) {
      setError(reviewError.message || "Could not update this post.");
    }
  }

  async function handleReviewAdoptionPost(id, decision) {
    if (!currentUser?.token || currentUser.role !== "Admin") {
      setError("Only Admin accounts can review adoption posts.");
      return;
    }

    try {
      const updatedPost = decision === "approve"
        ? await api.approveAdoptionPost(id, currentUser.token)
        : await api.rejectAdoptionPost(id, currentUser.token);

      setAdminAdoptions((current) =>
        current.map((item) => (item.id === id ? updatedPost : item))
      );
      setAdoptionNotice(decision === "approve" ? "Adoption post approved and published." : "Adoption post rejected.");
      setError("");
    } catch (reviewError) {
      setError(reviewError.message || "Could not update this adoption post.");
    }
  }

  async function handleDeleteAdoptionPost(id) {
    if (!currentUser?.token || currentUser.role !== "Admin") {
      setError("Only Admin accounts can delete adoption posts.");
      return;
    }

    try {
      await api.deleteAdoptionPost(id, currentUser.token);
      setAdminAdoptions((current) => current.filter((item) => item.id !== id));
      setAdoptions((current) => current.filter((item) => item.id !== id));
      setAdoptionNotice("Adoption post deleted.");
      setError("");
    } catch (deleteError) {
      setError(deleteError.message || "Could not delete this adoption post.");
    }
  }

  async function handleDeleteCommunityReport(kind, id) {
    if (!currentUser?.token || currentUser.role !== "Admin") {
      setError("Only Admin accounts can delete lost and found posts.");
      return;
    }

    try {
      if (kind === "lost") {
        await api.deleteLostPetReport(id, currentUser.token);
        setLostPets((current) => current.filter((item) => item.id !== id));
        setPendingLostPets((current) => current.filter((item) => item.id !== id));
      } else {
        await api.deleteFoundPetReport(id, currentUser.token);
        setFoundPets((current) => current.filter((item) => item.id !== id));
        setPendingFoundPets((current) => current.filter((item) => item.id !== id));
      }

      setLostFoundNotice("Post deleted.");
      setError("");
    } catch (deleteError) {
      setError(deleteError.message || "Could not delete this post.");
    }
  }

  async function refreshChatLists(preferredConversationId = null) {
    if (!currentUser?.token) {
      return;
    }

    const [conversations, vets] = await Promise.all([
      api.getMyChatConversations(currentUser.token),
      currentUser.role === "User" ? api.getChatVets(currentUser.token) : Promise.resolve([])
    ]);

    setChatConversations(conversations);
    setChatVets(vets);
    setSelectedConversationId((current) => {
      if (preferredConversationId && conversations.some((item) => item.id === preferredConversationId)) {
        return preferredConversationId;
      }

      if (current && conversations.some((item) => item.id === current)) {
        return current;
      }

      return conversations[0]?.id ?? null;
    });
  }

  async function handleStartChatWithVet(vetId) {
    if (!currentUser?.token) {
      setError("Please sign in first.");
      return;
    }

    try {
      const conversation = await api.createChatConversation(vetId, currentUser.token);
      await refreshChatLists(conversation.id);
      setChatNotice("Chat opened successfully.");
      setError("");
      setActiveTab("chat");
    } catch (requestError) {
      setError(requestError.message || "Could not start chat with this vet.");
    }
  }

  async function handleSendChatMessage(event) {
    event.preventDefault();

    if (!currentUser?.token || !selectedConversationId) {
      setError("Choose a chat first.");
      return;
    }

    const text = chatMessageDraft.trim();
    if (!text) {
      return;
    }

    try {
      const message = await api.sendChatMessage(selectedConversationId, text, currentUser.token);
      setChatMessages((current) => [...current, message]);
      setChatMessageDraft("");
      setChatNotice("");
      await refreshChatLists(selectedConversationId);
      setError("");
    } catch (requestError) {
      setError(requestError.message || "Could not send message.");
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
          <div className="sidebar-title-row">
            <h1>PetCare Jordan</h1>
            {currentUser && isChatRole ? (
              <button type="button" className="chat-bell-icon-button" onClick={() => setActiveTab("chat")}>
                <span className="chat-bell-emoji" aria-hidden="true">🔔</span>
                <strong className="chat-bell-badge">{chatUnreadCount}</strong>
              </button>
            ) : null}
          </div>
          <p className="sidebar-copy">
            A pet adoption, recovery, and veterinary care platform built with ASP.NET Core and React.
          </p>
        </div>

        <nav className="tab-list">
          {visibleTabs.map((tab) => (
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
                  <JordanPetsMap petsByCity={dashboard.petsByCity} />
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
              <div className="content-grid">
                {currentUser.role === "Admin" ? (
                  <>
                    <SectionCard title="Pending Adoption Posts" subtitle="Approve posts to publish them, or reject posts that should stay hidden.">
                      <div className="list-stack">
                        {pendingAdoptions.length > 0 ? (
                          pendingAdoptions.map((item) => (
                            <article key={item.id} className="list-card">
                              <strong>{item.petName}</strong>
                              <p>{item.story}</p>
                              <div className="meta-line">
                                <span>{item.petType}</span>
                                <span>{item.city}</span>
                                <span>{item.weightKg} kg</span>
                              </div>
                              <div className="meta-line">
                                <span>{item.contactMethod}: {item.contactDetails}</span>
                              </div>
                              <div className="form-grid-two">
                                <button type="button" className="admin-action-button" onClick={() => handleReviewAdoptionPost(item.id, "approve")}>
                                  Approve
                                </button>
                                <button type="button" className="admin-action-button" onClick={() => handleReviewAdoptionPost(item.id, "reject")}>
                                  Reject
                                </button>
                              </div>
                            </article>
                          ))
                        ) : (
                          <p className="empty-state">No pending adoption posts.</p>
                        )}
                      </div>
                    </SectionCard>

                    <SectionCard title="Published Adoption Posts" subtitle="Approved adoption posts visible to User and Vet accounts.">
                      <div className="list-stack">
                        {publishedAdoptions.length > 0 ? (
                          publishedAdoptions.map((item) => (
                            <article key={item.id} className="list-card">
                              <strong>{item.petName}</strong>
                              <p>{item.story}</p>
                              <div className="meta-line">
                                <span>{item.petType}</span>
                                <span>{item.city}</span>
                                <span>{item.weightKg} kg</span>
                              </div>
                              <div className="meta-line">
                                <span>{item.contactMethod}: {item.contactDetails}</span>
                              </div>
                              <button type="button" className="admin-action-button" onClick={() => handleDeleteAdoptionPost(item.id)}>
                                Delete
                              </button>
                            </article>
                          ))
                        ) : (
                          <p className="empty-state">No published adoption posts.</p>
                        )}
                      </div>
                    </SectionCard>

                    <SectionCard title="Rejected Adoption Posts" subtitle="Rejected posts stay hidden from users and vets.">
                      <div className="list-stack">
                        {rejectedAdoptions.length > 0 ? (
                          rejectedAdoptions.map((item) => (
                            <article key={item.id} className="list-card">
                              <strong>{item.petName}</strong>
                              <p>{item.story}</p>
                              <div className="meta-line">
                                <span>{item.petType}</span>
                                <span>{item.city}</span>
                              </div>
                              <button type="button" className="admin-action-button" onClick={() => handleDeleteAdoptionPost(item.id)}>
                                Delete
                              </button>
                            </article>
                          ))
                        ) : (
                          <p className="empty-state">No rejected adoption posts.</p>
                        )}
                      </div>
                    </SectionCard>
                  </>
                ) : (
                  <SectionCard title="Publish Adoption Post" subtitle="User and Vet accounts can add a pet with direct owner contact.">
                    {canPublishCommunityPost ? (
                      <form className="post-form adoption-form" onSubmit={handleCreateAdoptionPost}>
                        <div className="form-grid-two">
                          <input
                            type="text"
                            placeholder="Pet name"
                            value={adoptionPostForm.petName}
                            onChange={(event) => setAdoptionPostForm((current) => ({ ...current, petName: event.target.value }))}
                            required
                          />
                          <select
                            value={adoptionPostForm.petType}
                            onChange={(event) => setAdoptionPostForm((current) => ({ ...current, petType: event.target.value }))}
                          >
                            {petTypeOptions.map((type) => (
                              <option key={type} value={type}>
                                {type}
                              </option>
                            ))}
                          </select>
                        </div>
                        <input
                          type="number"
                          min="0.1"
                          step="0.1"
                          placeholder="Weight in kg"
                          value={adoptionPostForm.weightKg}
                          onChange={(event) => setAdoptionPostForm((current) => ({ ...current, weightKg: event.target.value }))}
                          required
                        />
                        <input
                          type="text"
                          placeholder="City or area"
                          value={adoptionPostForm.city}
                          onChange={(event) => setAdoptionPostForm((current) => ({ ...current, city: event.target.value }))}
                          required
                        />
                        <textarea
                          placeholder="Simple description"
                          value={adoptionPostForm.description}
                          onChange={(event) => setAdoptionPostForm((current) => ({ ...current, description: event.target.value }))}
                          required
                        />
                        <input
                          type="url"
                          placeholder="Photo URL (optional if you upload from device)"
                          value={adoptionPostForm.photoUrl}
                          onChange={(event) => setAdoptionPostForm((current) => ({ ...current, photoUrl: event.target.value }))}
                        />
                        <input
                          type="file"
                          accept="image/*"
                          onChange={(event) =>
                            handleUploadCommunityPhoto(
                              event.target.files?.[0],
                              setAdoptionPostForm,
                              setAdoptionPhotoUploading,
                              setAdoptionNotice
                            )
                          }
                        />
                        <p className="upload-hint">
                          {adoptionPhotoUploading
                            ? "Uploading image..."
                            : adoptionPostForm.photoUrl
                            ? "Image is ready. You can submit the adoption post."
                            : "Choose an image from your device, or paste a Photo URL."}
                        </p>
                        <input
                          type="text"
                          placeholder="Owner contact phone"
                          value={adoptionPostForm.contactPhone}
                          onChange={(event) => setAdoptionPostForm((current) => ({ ...current, contactPhone: event.target.value }))}
                          required
                        />
                        <button type="submit">Submit Adoption Post</button>
                      </form>
                    ) : (
                      <p className="empty-state">Only User and Vet accounts can publish adoption posts.</p>
                    )}
                  </SectionCard>
                )}

                {adoptionNotice ? <p className="form-success">{adoptionNotice}</p> : null}

                {currentUser.role !== "Admin" ? (
                <SectionCard title="Adoption Marketplace" subtitle="Owners can publish pets for adoption and adopters can contact them directly.">
                  <div className="pet-grid">
                    {adoptions.length > 0 ? (
                      adoptions.map((item) => (
                        <article key={item.id} className="pet-card">
                          {brokenAdoptionImages[item.id] ? (
                            <div className="pet-image-fallback">Photo unavailable</div>
                          ) : (
                            <img
                              src={item.photoUrl}
                              alt={item.petName}
                              onError={() => setBrokenAdoptionImages((current) => ({ ...current, [item.id]: true }))}
                            />
                          )}
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
                              <span>{item.weightKg} kg</span>
                            </div>
                            <div className="meta-line">
                              <span>{item.contactMethod}: {item.contactDetails}</span>
                            </div>
                          </div>
                        </article>
                      ))
                    ) : (
                      <p className="empty-state">No adoption posts yet.</p>
                    )}
                  </div>
                </SectionCard>
                ) : null}
              </div>
            ) : null}

            {activeTab === "lostfound" && currentUser && !privateLoading ? (
              <div className="content-grid">
                {currentUser.role === "Admin" ? (
                  <>
                    <SectionCard title="Pending Lost Pet Posts" subtitle="Approve posts to publish them, or reject posts that should stay hidden.">
                      <div className="list-stack">
                        {pendingLostPets.length > 0 ? (
                          pendingLostPets.map((item) => (
                            <article key={item.id} className="list-card">
                              <strong>{item.petName}</strong>
                              <p>{item.description}</p>
                              <div className="meta-line">
                                <span>{item.petType}</span>
                                <span>{item.lastSeenPlace}</span>
                                <span>{new Date(item.lastSeenDateUtc).toLocaleDateString()}</span>
                              </div>
                              <div className="meta-line">
                                <span>Contact: {item.contactName}</span>
                                <span>{item.contactPhone}</span>
                              </div>
                              <div className="form-grid-two">
                                <button type="button" className="admin-action-button" onClick={() => handleReviewCommunityReport("lost", item.id, "approve")}>
                                  Approve
                                </button>
                                <button type="button" className="admin-action-button" onClick={() => handleReviewCommunityReport("lost", item.id, "reject")}>
                                  Reject
                                </button>
                              </div>
                            </article>
                          ))
                        ) : (
                          <p className="empty-state">No pending lost pet posts.</p>
                        )}
                      </div>
                    </SectionCard>

                    <SectionCard title="Pending Found Pet Posts" subtitle="Found pet reports also stay hidden until admin approval.">
                      <div className="list-stack">
                        {pendingFoundPets.length > 0 ? (
                          pendingFoundPets.map((item) => (
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
                              <div className="form-grid-two">
                                <button type="button" className="admin-action-button" onClick={() => handleReviewCommunityReport("found", item.id, "approve")}>
                                  Approve
                                </button>
                                <button type="button" className="admin-action-button" onClick={() => handleReviewCommunityReport("found", item.id, "reject")}>
                                  Reject
                                </button>
                              </div>
                            </article>
                          ))
                        ) : (
                          <p className="empty-state">No pending found pet posts.</p>
                        )}
                      </div>
                    </SectionCard>

                    <SectionCard title="Published Lost Pet Posts" subtitle="Already approved posts visible to User and Vet accounts.">
                      <div className="list-stack">
                        {lostPets.length > 0 ? (
                          lostPets.map((item) => (
                            <article key={item.id} className="list-card">
                              <strong>{item.petName}</strong>
                              <p>{item.description}</p>
                              <div className="meta-line">
                                <span>{item.petType}</span>
                                <span>{item.lastSeenPlace}</span>
                                <span>{new Date(item.lastSeenDateUtc).toLocaleDateString()}</span>
                              </div>
                              <div className="meta-line">
                                <span>Contact: {item.contactName}</span>
                                <span>{item.contactPhone}</span>
                              </div>
                              <button type="button" className="admin-action-button" onClick={() => handleDeleteCommunityReport("lost", item.id)}>
                                Delete
                              </button>
                            </article>
                          ))
                        ) : (
                          <p className="empty-state">No published lost pet posts.</p>
                        )}
                      </div>
                    </SectionCard>

                    <SectionCard title="Published Found Pet Posts" subtitle="Remove found pet posts that should no longer appear publicly.">
                      <div className="list-stack">
                        {foundPets.length > 0 ? (
                          foundPets.map((item) => (
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
                              <button type="button" className="admin-action-button" onClick={() => handleDeleteCommunityReport("found", item.id)}>
                                Delete
                              </button>
                            </article>
                          ))
                        ) : (
                          <p className="empty-state">No published found pet posts.</p>
                        )}
                      </div>
                    </SectionCard>
                  </>
                ) : (
                  <>
                    <SectionCard title="My Posts" subtitle="Your lost and found reports stay here, separate from other people's posts.">
                      <div className="content-grid two-column">
                        <div className="list-stack">
                          <strong>My Lost Reports</strong>
                          {myLostPets.length > 0 ? (
                            myLostPets.map((item) => (
                              <article key={item.id} className="list-card">
                                <strong>{item.petName}</strong>
                                <p>{item.description}</p>
                                <div className="meta-line">
                                  <span>{item.lastSeenPlace}</span>
                                  <span>{new Date(item.lastSeenDateUtc).toLocaleDateString()}</span>
                                </div>
                                <div className="meta-line">
                                  <span className="pill warning">{item.status}</span>
                                  <span>Reward: {item.rewardAmount ? `${item.rewardAmount} JOD` : "No reward listed"}</span>
                                </div>
                              </article>
                            ))
                          ) : (
                            <p className="empty-state">You have not submitted lost pet reports yet.</p>
                          )}
                        </div>

                        <div className="list-stack">
                          <strong>My Found Reports</strong>
                          {myFoundPets.length > 0 ? (
                            myFoundPets.map((item) => (
                              <article key={item.id} className="list-card">
                                <strong>{item.petType}</strong>
                                <p>{item.description}</p>
                                <div className="meta-line">
                                  <span>{item.foundPlace}</span>
                                  <span>{new Date(item.foundDateUtc).toLocaleDateString()}</span>
                                </div>
                                <div className="meta-line">
                                  <span className="pill warning">{item.status}</span>
                                  <span>{item.contactPhone}</span>
                                </div>
                              </article>
                            ))
                          ) : (
                            <p className="empty-state">You have not submitted found pet reports yet.</p>
                          )}
                        </div>
                      </div>
                    </SectionCard>

                    <div className="content-grid two-column">
                      <SectionCard title="Community Lost Pets" subtitle="Approved missing-pet posts from other accounts.">
                        <div className="list-stack">
                          {communityLostPets.length > 0 ? (
                            communityLostPets.map((item) => (
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
                            ))
                          ) : (
                            <p className="empty-state">No approved lost pet posts from other accounts.</p>
                          )}
                        </div>
                      </SectionCard>

                      <SectionCard title="Community Found Pets" subtitle="Approved found-pet posts from other accounts.">
                        <div className="list-stack">
                          {communityFoundPets.length > 0 ? (
                            communityFoundPets.map((item) => (
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
                            ))
                          ) : (
                            <p className="empty-state">No approved found pet posts from other accounts.</p>
                          )}
                        </div>
                      </SectionCard>
                    </div>

                    <SectionCard title="Publish Lost / Found Report" subtitle="Your post is saved as Pending and appears publicly only after admin approval.">
                      {canPublishCommunityPost ? (
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
                        <p className="empty-state">Only User and Vet accounts can publish lost or found reports.</p>
                      )}
                    </SectionCard>
                  </>
                )}

                {lostFoundNotice ? <p className="form-success">{lostFoundNotice}</p> : null}
              </div>
            ) : null}

            {activeTab === "chat" && currentUser && !privateLoading ? (
              <SectionCard
                title="Vet Chat"
                subtitle="Choose a vet and start a real conversation. Vets can reply from their own accounts."
              >
                {currentUser.role === "User" || currentUser.role === "Vet" ? (
                  <div className="chat-layout">
                    <aside className="chat-sidebar">
                      {currentUser.role === "User" ? (
                        <div className="chat-start-card">
                          <strong>Start new chat</strong>
                          {vetsWithoutConversation.length > 0 ? (
                            <div className="chat-vet-list">
                              {vetsWithoutConversation.map((vet) => (
                                <button key={vet.id} type="button" onClick={() => handleStartChatWithVet(vet.id)}>
                                  <span>{vet.fullName}</span>
                                  <small>{vet.city}</small>
                                </button>
                              ))}
                            </div>
                          ) : (
                            <p className="empty-state">You already started chats with all available vets.</p>
                          )}
                        </div>
                      ) : null}

                      <div className="chat-start-card">
                        <strong>{currentUser.role === "User" ? "My chats" : "User chats"}</strong>
                        {chatConversations.length > 0 ? (
                          <div className="chat-vet-list">
                            {chatConversations.map((conversation) => (
                              <button
                                key={conversation.id}
                                type="button"
                                className={selectedConversationId === conversation.id ? "active" : ""}
                                onClick={() => setSelectedConversationId(conversation.id)}
                              >
                                <div className="chat-conversation-head">
                                  <span>{conversation.counterpartName}</span>
                                  {conversation.unreadIncomingCount > 0 ? (
                                    <strong className="chat-unread-pill">{conversation.unreadIncomingCount}</strong>
                                  ) : null}
                                </div>
                                <small>{conversation.lastMessage || "No messages yet."}</small>
                              </button>
                            ))}
                          </div>
                        ) : (
                          <p className="empty-state">No chats yet.</p>
                        )}
                      </div>
                    </aside>

                    <section className="chat-thread">
                      {selectedConversation ? (
                        <>
                          <div className="chat-thread-head">
                            <strong>{selectedConversation.counterpartName}</strong>
                            <span>
                              {selectedConversation.counterpartRole === "Vet" ? "Veterinarian" : "Pet Owner"}
                            </span>
                          </div>

                          <div className="chat-messages-list">
                            {chatLoading ? <p className="empty-state">Loading messages...</p> : null}
                            {!chatLoading && chatMessages.length === 0 ? (
                              <p className="empty-state">No messages yet. Start the conversation now.</p>
                            ) : null}
                            {chatMessages.map((message) => (
                              <article
                                key={message.id}
                                className={message.senderId === currentUser.id ? "chat-message own" : "chat-message"}
                              >
                                <div className="chat-message-meta">
                                  <strong>{message.senderName}</strong>
                                  <span>{formatJordanDateTime(message.sentAtUtc)}</span>
                                </div>
                                <p>{message.message}</p>
                              </article>
                            ))}
                          </div>

                          <form className="chat-compose" onSubmit={handleSendChatMessage}>
                            <textarea
                              placeholder="Write your message..."
                              value={chatMessageDraft}
                              onChange={(event) => setChatMessageDraft(event.target.value)}
                            />
                            <button type="submit">Send</button>
                          </form>
                        </>
                      ) : (
                        <p className="empty-state">Choose a chat from the left side to start messaging.</p>
                      )}
                    </section>
                  </div>
                ) : (
                  <p className="empty-state">Chat is available for User and Vet accounts only.</p>
                )}

                {chatNotice ? <p className="form-success">{chatNotice}</p> : null}
              </SectionCard>
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
