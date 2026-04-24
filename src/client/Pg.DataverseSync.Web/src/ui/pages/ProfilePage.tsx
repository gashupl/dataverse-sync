import { useProfile } from '../../application/hooks/useProfile';
import { useNavigate } from 'react-router-dom';

/**
 * Profile page component
 * Displays current user's profile information
 */
export function ProfilePage() {
  const { user, isLoading, error } = useProfile();
  const navigate = useNavigate();

  if (isLoading) {
    return (
      <section className="profile-page">
        <p>Loading profile...</p>
      </section>
    );
  }

  if (error || !user) {
    return (
      <section className="profile-page">
        <p className="profile-error">{error ?? 'User not found.'}</p>
        <button className="profile-back-btn" onClick={() => navigate('/')}>
          Back to Home
        </button>
      </section>
    );
  }

  return (
    <section className="profile-page">
      <h2 className="profile-heading">Profile</h2>
      <div className="profile-card">
        <div className="profile-field">
          <span className="profile-label">Username</span>
          <span className="profile-value">{user.username}</span>
        </div>
        <div className="profile-field">
          <span className="profile-label">Email</span>
          <span className="profile-value">{user.email}</span>
        </div>
        <div className="profile-field">
          <span className="profile-label">Created on</span>
          <span className="profile-value">
            {new Date(user.createdOn).toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: 'numeric' })}
          </span>
        </div>
      </div>
      <button className="profile-back-btn" onClick={() => navigate('/')}>
        Back to Home
      </button>
    </section>
  );
}
