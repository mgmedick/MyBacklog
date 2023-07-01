import { createApp } from "vue";

import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { faUser, faMoon, faGear, faRightFromBracket, faClipboard, faHourglassEnd, faCircleCheck, faSpinner, faSquarePen, faAt, faList, faInbox, faPlay, faCheck, faEllipsis, faPlus } from '@fortawesome/free-solid-svg-icons'
import { faGoogle, faFacebook, faSteam, faXbox } from '@fortawesome/free-brands-svg-icons'
library.add(faUser, faMoon, faGear, faRightFromBracket, faClipboard, faHourglassEnd, faCircleCheck, faSpinner, faSquarePen, faAt, faList, faInbox, faPlay, faCheck, faEllipsis, faPlus, faGoogle, faFacebook, faSteam, faXbox);

import buttonDropdown from './shared/ButtonDropdown.vue';
import autocomplete from './shared/Autocomplete.vue';
import multiselect from './shared/Multiselect.vue';
import modal from './shared/Modal.vue';
import navbar from './menu/Navbar.vue';

import index from './home/Index.vue';
import signUp from './home/SignUp.vue';
import activate from './home/Activate.vue';
import login from './home/Login.vue';
import resetPassword from './home/ResetPassword.vue';
import changePassword from './home/ChangePassword.vue';
import changeUsername from './home/ChangeUsername.vue';
import welcome from './home/Welcome.vue';

import userSettings from './user/UserSettings.vue';
import userGameLists from './user/UserGameLists.vue';
import userGameListGames from './user/UserGameListGames.vue';

export default {
    loadComponents() {
        const app = createApp({
            // components: {
            //     'font-awesome-icon': FontAwesomeIcon
            // }
        });
        
        app.component("font-awesome-icon", FontAwesomeIcon);
        app.component("button-dropdown", buttonDropdown);
        app.component("navbar", navbar);
        app.component('autocomplete', autocomplete);
        app.component('multiselect', multiselect);
        app.component('modal', modal);

        app.component('index', index); 

        app.component('user-settings', userSettings); 
        app.component('user-gamelists', userGameLists); 
        app.component('user-gamelist-games', userGameListGames); 

        app.component("reset-password", resetPassword);
        app.component("change-password", changePassword);
        app.component("change-username", changeUsername);
        app.component("login", login);
        app.component("signup", signUp);
        app.component("activate", activate);
        app.component("welcome", welcome);

        app.mount('#vue-app');
        app.provide('app', 'Vue3');
    }
}




