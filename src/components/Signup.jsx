// components/SignUpForm.jsx
import React from "react";
import InputField from "./InputField";

const SignUpForm = () => {
  return (
    <form className="signup-form">
      <h2>Sign Up</h2>

      <InputField type="text" placeholder="Full Name" icon="person" />
      <InputField type="email" placeholder="Email Address" icon="mail" />
      <InputField type="password" placeholder="Password" icon="lock" />
      <InputField type="password" placeholder="Confirm Password" icon="lock" />

      <button type="submit" className="submit-button">Register</button>
    </form>
  );
};

export default SignUpForm;
