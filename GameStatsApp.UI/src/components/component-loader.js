import { createApp } from "vue";
import VueTippy from "vue-tippy";

import buttonDropdown from './shared/ButtonDropdown.vue';
import autocomplete from './shared/Autocomplete.vue';
import multiselect from './shared/Multiselect.vue';
import modal from './shared/Modal.vue';
import navbar from './menu/Navbar.vue';

import userDetails from './user/UserDetails.vue';
import signUp from './user/SignUp.vue';
import activate from './user/Activate.vue';
import login from './user/Login.vue';
import resetPassword from './user/ResetPassword.vue';
import changePassword from './user/ChangePassword.vue';

export default {
    loadComponents() {
        const app = createApp({
            components: {
                //'speedrun-list-tab': speedRunListTab
            }
        })
        .use(VueTippy, { defaultProps: { allowHTML: true } });
        
        app.component("button-dropdown", buttonDropdown);
        app.component("navbar", navbar);
        app.component('autocomplete', autocomplete);
        app.component('multiselect', multiselect);
        app.component('modal', modal);

        app.component('userdetails', userDetails); 
        app.component("reset-password", resetPassword);
        app.component("change-password", changePassword);
        app.component("login", login);
        app.component("signup", signUp);
        app.component("activate", activate);

        app.mount('#vue-app');
        app.provide('app', 'Vue3');
    }
}




