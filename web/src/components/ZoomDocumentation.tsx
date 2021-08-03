import { routes } from "./App";

export const ZoomDocumentation = () => {
  return (
    <div>
      <h1 id="Zoom-Integration" data-renderer-start-pos="1">
        <strong data-renderer-mark="true">Zoom Integration</strong>
      </h1>
      <h3 id="1-Installation" data-renderer-start-pos="19">
        1 Installation
      </h3>
      <ol data-indent-level="1">
        <li>
          <p data-renderer-start-pos="37">Go to the Talk2Me Landing page → Press ‘Log in via Zoom’ button</p>
        </li>
        <li>
          <p data-renderer-start-pos="104">The system will redirect you to the Zoom log in → Enter your credentials and log in to Zoom</p>
        </li>
        <li>
          <p data-renderer-start-pos="199">When you successfully log in via Zoom, you will be redirected to the ‘Find a Room’ page.</p>
        </li>
      </ol>
      <p data-renderer-start-pos="291">If you have any issues with it, please
        <strong data-renderer-mark="true">
          <a href={routes.help.contactUs}> contact us here.</a>
        </strong>
      </p>
      <h3 id="2-Usage:-use-Zoom-for-calls" data-renderer-start-pos="348">
        2 Usage<strong data-renderer-mark="true">: use Zoom for calls</strong>
      </h3>
      <ol data-indent-level="1">
        <li>
          <p data-renderer-start-pos="379">You must be logged in to the Talk2Me app in order to use Zoom for calls.</p>
        </li>
        <li>
          <p data-renderer-start-pos="455">Join existent room or create a new one.</p>
        </li>
        <li>
          <p data-renderer-start-pos="498">Go to ‘My Rooms’ page.</p>
        </li>
        <li>
          <p data-renderer-start-pos="524">Press ‘Join a room’ button (it becomes available 5 minutes before the set time) → the system will redirect you to Zoom so you can have a call.</p>
        </li>
      </ol>
      <p data-renderer-start-pos="670">If you have any issues with it, please
        <strong data-renderer-mark="true">
          <a href={routes.help.contactUs}> contact us here.</a>
        </strong>
      </p>
      <h3 id="3-Uninstallation" data-renderer-start-pos="727">
        3 Uninstallation
      </h3>
      <ol data-indent-level="1">
        <li>
          <p data-renderer-start-pos="747">Log in to your Zoom account → go to the Zoom App Marketplace.</p>
        </li>
        <li>
          <p data-renderer-start-pos="812">Click ‘Manage’ → Installed Apps or Search for the Talk2Me App.</p>
        </li>
        <li>
          <p data-renderer-start-pos="878">Click ‘Talk2Me App’.</p>
        </li>
        <li>
          <p data-renderer-start-pos="902">Click ‘Uninstall’.</p>
        </li>
      </ol>
      <p data-renderer-start-pos="924">If you have any issues with it, please
        <strong data-renderer-mark="true">
          <a href={routes.help.contactUs}> contact us here.</a>
        </strong>
      </p>
    </div>
  );
}