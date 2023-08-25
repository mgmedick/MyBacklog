<template>
    <div class="mx-auto">
        <h2 class="text-center mb-1">User Settings</h2>
        <div class="mx-auto" style="max-width:400px;">
            <div class="mb-4">
                <change-username :username="usersettingsvm.username"></change-username>
            </div>
            <div class="text-center">
                <h4>Link Accounts</h4>
            </div>
            <div class="row g-2 justify-content-center mb-4">
                <button type="button" class="btn btn-outline-primary d-flex" :disabled="usersettingsvm.accountTypeIDs.indexOf(1) > -1"><font-awesome-icon icon="fa-brands fa-steam" size="xl" style="color: #0a3169;" /><span class="mx-auto">Link Steam account</span><font-awesome-icon v-if="usersettingsvm.accountTypeIDs.indexOf(1) > -1" icon="fa-solid fa-circle-check" size="xl" style="color: #02b875;"/></button>
                <button type="button" class="btn btn-outline-primary d-flex" :disabled="usersettingsvm.accountTypeIDs.indexOf(2) > -1" @click="onXboxClick"><font-awesome-icon icon="fa-brands fa-xbox" size="xl" style="color: #107711;" /><span class="mx-auto">Link Xbox account</span><font-awesome-icon v-if="usersettingsvm.accountTypeIDs.indexOf(2) > -1" icon="fa-solid fa-circle-check" size="xl" style="color: #02b875;"/></button>
            </div>
            <div class="text-center">
                <h4>Manage Lists</h4>
            </div>   
            <manage-user-lists></manage-user-lists>
        </div>                             
    </div>
</template>
<script>
    import { successToast, errorToast } from '../../js/common.js';
    
    export default {
        name: "UserSettings",
        props: {
            usersettingsvm: Object
        },
        data() {
            return {
                successMessage: '',
                errorMessages: [],
                addListModal: {},
                editListModal: {},
                deleteListModal: {},
                userList: {}
            }
        },
        computed: {
        },
        mounted: function () {
            var that = this;

            if (that.usersettingsvm.authSuccess != null) {
                if (that.usersettingsvm.authSuccess) {
                    successToast("Successfully linked account");             
                } else {                 
                    errorToast("Error linking account");             
                }
            }           
        },
        methods: {            
            onXboxClick() {
                location.href = this.usersettingsvm.windowsLiveAuthUrl;
            },            
            onContinueClick() {
                location.href = "/";
            }           
        }    
    };
</script>


