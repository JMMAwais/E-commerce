import { useState } from "react";
import SocialLogin from "./components/SocialLogin";
import InputField from "./components/InputField";

const App = () => {
  const [isSignUp, setIsSignUp] = useState(false);

  const toggleForm = () => {
    setIsSignUp(prev => !prev);
  };

  return (
    <div className="login-container">
      <h2 className="form-title">{isSignUp ? "Sign up with" : "Log in with"}</h2>

      <SocialLogin />
      <p className="separator"><span>or</span></p>

      <form action="#" className="login-form">
        {isSignUp && (
          <InputField type="text" placeholder="Full Name" icon="person" />
        )}
        <InputField type="email" placeholder="Email address" icon="mail" />
        <InputField type="password" placeholder="Password" icon="lock" />
        {isSignUp && (
          <InputField type="password" placeholder="Confirm Password" icon="lock" />
        )}

        {!isSignUp && (
          <a href="#" className="forgot-password-link">Forgot password?</a>
        )}

        <button type="submit" className="login-button">
          {isSignUp ? "Sign Up" : "Log In"}
        </button>
      </form>

      <p className="signup-prompt">
        {isSignUp ? (
          <>Already have an account? <a href="#" className="signup-link" onClick={toggleForm}>Log in</a></>
        ) : (
          <>Don&apos;t have an account? <a href="#" className="signup-link" onClick={toggleForm}>Sign up</a></>
        )}
      </p>
    </div>
  );
};

export default App;
