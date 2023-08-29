<template>
    <div class="mx-auto">
        <h3 class="text-center mb-1">Welcome to GameStatsApp</h3>
        <div class="mx-auto" style="max-width:400px;">
            <div class="mb-3">
                <change-username :username="username"></change-username>
            </div>      
            <div class="text-center">
                <p class="lead text-dark">Link an account to import games</p>
            </div>
            <div class="row g-2 justify-content-center mb-3">
                <button type="button" class="btn btn-outline-primary d-flex" :disabled="useraccounts?.filter(i => i.accountTypeID == 1).length > 0"><font-awesome-icon icon="fa-brands fa-steam" size="xl" style="color: #0a3169;" /><span class="mx-auto">Link Steam account</span><font-awesome-icon v-if="useraccounts?.filter(i => i.accountTypeID == 1).length > 0" icon="fa-solid fa-circle-check" size="xl" style="color: #02b875;"/></button>
                <button type="button" class="btn btn-outline-primary d-flex" :disabled="useraccounts?.filter(i => i.accountTypeID == 2).length > 0" @click="onXboxClick"><font-awesome-icon icon="fa-brands fa-xbox" size="xl" style="color: #107711;" /><span class="mx-auto">Link Xbox account</span><font-awesome-icon v-if="useraccounts?.filter(i => i.accountTypeID == 2).length > 0" icon="fa-solid fa-circle-check" size="xl" style="color: #02b875;"/></button>
            </div> 
            <div class="text-center">
                <span class="lead text-dark">Info:</span>
            </div>            
            <div class="mb-3">
                <ul class="p-0">
                    <li>Add new games using <font-awesome-icon icon="fa-solid fa-plus"/></li>
                    <li>Add/Remove games from lists using <font-awesome-icon icon="fa-solid fa-list"/>&nbsp;<font-awesome-icon icon="fa-solid fa-inbox"/>&nbsp;<font-awesome-icon icon="fa-solid fa-play"/>&nbsp;<font-awesome-icon icon="fa-solid fa-check"/>&nbsp;<font-awesome-icon icon="fa-solid fa-ellipsis"/></li>
                    <li>Import games using <font-awesome-icon icon="fa-solid fa-cloud-arrow-down"/></li>
                    <li>Manage lists and link accounts from Settings <font-awesome-icon icon="fa-solid fa-gear"/></li>
                </ul>
            </div>            
        </div>
    </div>
</template>
<script>
    import { successToast, errorToast } from '../../js/common.js';
    
    export default {
        name: "Welcome",
        props: {
            username: String,
            useraccounts: Array,
            windowsliveauthurl: String,
            authsuccess: Boolean,
            authaccounttypeid: Number
        },
        data() {
            return {
            }
        },
        computed: {
        },
        mounted: function () {
            var that = this;

            if (that.authsuccess != null) {
                var userAccount = that.useraccounts.find(i => i.accountTypeID == that.authaccounttypeid);

                if (that.authsuccess) {
                    successToast("Successfully linked " + userAccount.accountTypeName + " account");                               
                } else {                 
                    errorToast("Error linking account");             
                }
            }            
        },
        methods: {            
            onXboxClick() {
                location.href = this.windowsliveauthurl;
            },            
            onContinueClick() {
                location.href = "/";
            }    
        }
    };
</script>


