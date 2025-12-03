import React from 'react';
import { useNavigate } from 'react-router-dom';
import styles from '../styles/Navbar.module.css';
import CharacterBuilder from './CharacterBuilder';

interface NavbarProps {
  username?: string;
  onOpenCharacterBuilder?: () => void;
  showCharacterButton?: boolean;
  isEnding?: boolean;
}

const Navbar: React.FC<NavbarProps> = ({ 
  username,
  onOpenCharacterBuilder, 
  showCharacterButton = false,
  isEnding = false
}) => {
  const navigate = useNavigate();

  
  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

   const handleHomeClick = () => {
    navigate('/home');
  };

  return (
    <nav className={styles.navbar}>
      <div className={styles.navLeft}>
        <h1 className={styles.title} onClick={handleHomeClick}>The Fallen Dragon</h1>
        {username && <span className={styles.username}>Playing as: {username}</span>}
      </div>
      <div className={styles.navRight}>
        {showCharacterButton && onOpenCharacterBuilder && !isEnding && (
          <button 
            onClick={onOpenCharacterBuilder}
            className={styles.navButton}
          >
            Edit Character
          </button>
        )}
        <button onClick={handleLogout} className={styles.navButton}>
          Logout
        </button>
      </div>
    </nav>
  );
};

export default Navbar;
