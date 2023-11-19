import { createApp } from "vue";

import { library } from '@fortawesome/fontawesome-svg-core'
import { FontAwesomeIcon, FontAwesomeLayers } from '@fortawesome/vue-fontawesome'
import { faUser, faMoon, faGear, faRightFromBracket, faClipboard, faHourglassEnd, faCircleCheck, faSpinner, faSquarePen, faAt, faList, faInbox, faPlay, faCheck, faEllipsis, faPlus, faCircleXmark, faCloudArrowDown, faCircleExclamation, faArrowDownWideShort, faArrowUpWideShort, faCloud, faCircleInfo, faAngleUp, faAngleDown, faLayerGroup, faCube, faFilter, faTriangleExclamation, faEye, faEyeSlash, faEraser } from '@fortawesome/free-solid-svg-icons'
import { faGoogle, faFacebook, faSteam, faXbox } from '@fortawesome/free-brands-svg-icons'
library.add(faUser, faMoon, faGear, faRightFromBracket, faClipboard, faHourglassEnd, faCircleCheck, faSpinner, faSquarePen, faAt, faList, faInbox, faPlay, faCheck, faEllipsis, faPlus, faCircleXmark, faCloudArrowDown, faCircleExclamation, faArrowDownWideShort, faArrowUpWideShort, faCloud, faCircleInfo, faAngleUp, faAngleDown, faLayerGroup, faCube, faFilter, faTriangleExclamation, faEye, faEyeSlash, faEraser, faGoogle, faFacebook, faSteam, faXbox);

import autocomplete from './shared/Autocomplete.vue';
import multiselect from './shared/Multiselect.vue';
import navbar from './menu/Navbar.vue';
import about from './menu/About.vue';

import index from './home/Index.vue';
import signUp from './home/SignUp.vue';
import activate from './home/Activate.vue';
import login from './home/Login.vue';
import resetPassword from './home/ResetPassword.vue';
import changePassword from './home/ChangePassword.vue';
import changeUsername from './home/ChangeUsername.vue';
import welcome from './home/Welcome.vue';

import importGames from './game/ImportGames.vue';

import userSettings from './user/UserSettings.vue';

import userLists from './userlist/UserLists.vue';
import manageUserLists from './userlist/ManageUserLists.vue';
import editUserList from './userlist/EditUserList.vue';
import userListGames from './userlist/UserListGames.vue';


export default {
    loadComponents() {
        const app = createApp({
            // components: {
            //     'font-awesome-icon': FontAwesomeIcon
            // }
        });
        
        app.component("font-awesome-icon", FontAwesomeIcon);
        app.component("font-awesome-layers", FontAwesomeLayers);
        app.component("navbar", navbar);
        app.component("about", about);
        app.component('autocomplete', autocomplete);
        app.component('multiselect', multiselect);

        app.component('index', index); 
        app.component("reset-password", resetPassword);
        app.component("change-password", changePassword);
        app.component("change-username", changeUsername);
        app.component("login", login);
        app.component("signup", signUp);
        app.component("activate", activate);

        app.component("import-games", importGames);

        app.component('user-settings', userSettings); 

        app.component("welcome", welcome);
        app.component("manage-user-lists", manageUserLists);
        app.component("edit-user-list", editUserList);        
        app.component('user-lists', userLists); 
        app.component('user-list-games', userListGames);
        app.config.globalProperties.getCsrfToken = () => { 
            return document.getElementsByName("__RequestVerificationToken")[0].value; 
        }

        app.mount('#vue-app');
        app.provide('app', 'Vue3');
    }
}




